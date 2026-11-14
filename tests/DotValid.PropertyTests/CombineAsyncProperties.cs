using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Validated.CombineAsync</c> waits for two to eight validations that run as tasks and combines them like
/// <c>Combine</c> (V7, V10): every error of every input, in argument order. With dotResult's R8 semantics: missing
/// arguments throw at once, a failed task comes out when awaited (every task observed), a canceled one cancels the
/// result, and completed tasks give a completed result.
/// </summary>
public sealed class CombineAsyncProperties : IDisposable
{
    private readonly CancellationTokenSource _source = new();

    public CombineAsyncProperties() => _source.Cancel();

    public void Dispose() => _source.Dispose();

    [Property]
    public async Task Every_arity_is_awaiting_each_input_then_Combine(
        Validated<int, string> v1,
        Validated<int, string> v2,
        Validated<int, string> v3,
        Validated<int, string> v4,
        Validated<int, string> v5,
        Validated<int, string> v6,
        Validated<int, string> v7,
        Validated<int, string> v8,
        bool yields)
    {
        Validated<int, string>[] all = [v1, v2, v3, v4, v5, v6, v7, v8];
        for (var arity = 2; arity <= 8; arity++)
        {
            var inputs = all[..arity];
            var expected = Validations.Valid<int, string>(0);
            foreach (var input in inputs)
            {
                expected = expected.Zip(input, (sum, value) => sum + value);
            }

            Assert.Equal(expected, await CombineAllAsync([.. inputs.Select(input => Deferred.Lift(input, yields))], values => values.Sum()));
        }
    }

    [Fact]
    public async Task Completed_tasks_give_a_completed_result()
    {
        var done = Task.FromResult(Validations.Valid<int, string>(1));
        var failed = Task.FromResult(Validations.Invalid<int, string>("boom"));

        var combined = Validated.CombineAsync(done, failed, (a, b) => a + b);

        Assert.True(combined.IsCompletedSuccessfully);
        ValidatedAssert.IsInvalid(["boom"], await combined);
    }

    [Fact]
    public async Task A_failed_task_comes_out_when_awaited_and_every_task_is_observed()
    {
        var first = new InvalidOperationException("first");
        var second = new InvalidOperationException("second");
        var calls = 0;

        var pending = Validated.CombineAsync(
            Task.FromResult(Validations.Valid<int, string>(1)),
            Task.FromException<Validated<int, string>>(first),
            Task.FromException<Validated<int, string>>(second),
            (a, b, c) => ++calls);

        Assert.Same(first, await Assert.ThrowsAsync<InvalidOperationException>(async () => await pending));
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task A_task_that_fails_later_comes_out_as_itself()
    {
        var thrown = new InvalidOperationException("later");
        var later = Later(thrown);

        var pending = Validated.CombineAsync(Task.FromResult(Validations.Valid<int, string>(1)), later, (a, b) => a + b);

        Assert.Same(thrown, await Assert.ThrowsAsync<InvalidOperationException>(async () => await pending));

        static async Task<Validated<int, string>> Later(Exception exception)
        {
            await Task.Yield();
            throw exception;
        }
    }

    [Fact]
    public async Task A_canceled_task_cancels_the_result()
    {
        var task = Validated.CombineAsync(
            Task.FromResult(Validations.Valid<int, string>(1)),
            Task.FromCanceled<Validated<int, string>>(_source.Token),
            (a, b) => a + b).AsTask();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        Assert.True(task.IsCanceled);
    }

    [Fact]
    public async Task A_null_from_the_combiner_or_an_uninitialized_result_fails_when_awaited()
    {
        var done = Task.FromResult(Validations.Valid<int, string>(1));

        var nullResult = Validated.CombineAsync(done, done, (_, _) => (string)null!);
        var uninitialized = Validated.CombineAsync(done, Task.FromResult(default(Validated<int, string>)), (a, b) => a + b);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await nullResult);
        Assert.Equal("second", (await Assert.ThrowsAsync<ArgumentException>(async () => await uninitialized)).ParamName);
    }

    [Fact]
    public async Task Every_arity_gives_the_exception_of_a_failed_task_at_any_position_as_itself()
    {
        for (var arity = 2; arity <= 8; arity++)
        {
            for (var position = 0; position < arity; position++)
            {
                var thrown = new InvalidOperationException($"failed at {position}");
                var inputs = Enumerable.Repeat(Task.FromResult(Validations.Valid<int, string>(1)), arity).ToArray();
                inputs[position] = Task.FromException<Validated<int, string>>(thrown);

                var pending = CombineAllAsync(inputs, values => values.Sum());

                Assert.Same(thrown, await Assert.ThrowsAsync<InvalidOperationException>(async () => await pending));
            }
        }
    }

    [Fact]
    public async Task No_arity_resumes_on_the_callers_synchronization_context()
    {
        for (var arity = 2; arity <= 8; arity++)
        {
            var context = new CountingContext();
            var first = new TaskCompletionSource<Validated<int, string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var inputs = Enumerable.Repeat(Task.FromResult(Validations.Valid<int, string>(1)), arity).ToArray();
            inputs[0] = first.Task;
            ValueTask<Validated<int, string>> pending;

            var previous = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context);
            try
            {
                pending = CombineAllAsync(inputs, values => values.Sum());
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }

            first.SetResult(Validations.Valid<int, string>(1));

            Assert.Equal(Validations.Valid<int, string>(arity), await pending);
            Assert.Equal(0, context.Posts);
        }
    }

    [Fact]
    public void Every_arity_rejects_a_missing_task_by_its_name_and_a_missing_combiner()
    {
        string[] names = ["first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth"];
        for (var arity = 2; arity <= 8; arity++)
        {
            for (var position = 0; position < arity; position++)
            {
                var inputs = Enumerable.Repeat(Task.FromResult(Validations.Valid<int, string>(1)), arity).ToArray();
                inputs[position] = null!;

                Assert.Equal(names[position], Assert.Throws<ArgumentNullException>(() => CombineAllAsync(inputs, _ => 0)).ParamName);
            }

            var done = Enumerable.Repeat(Task.FromResult(Validations.Valid<int, string>(1)), arity).ToArray();
            Assert.Equal("combine", Assert.Throws<ArgumentNullException>(() => CombineAllAsync(done, null)).ParamName);
        }
    }

    // Calls the CombineAsync overload for inputs.Length (2..8); a null combine passes null to that overload.
    private static ValueTask<Validated<int, string>> CombineAllAsync(Task<Validated<int, string>>[] v, Func<int[], int>? combine) =>
        v.Length switch
        {
            2 => Validated.CombineAsync(v[0], v[1], combine is null ? null! : (Func<int, int, int>)((a, b) => combine([a, b]))),
            3 => Validated.CombineAsync(v[0], v[1], v[2], combine is null ? null! : (Func<int, int, int, int>)((a, b, c) => combine([a, b, c]))),
            4 => Validated.CombineAsync(
                v[0], v[1], v[2], v[3], combine is null ? null! : (Func<int, int, int, int, int>)((a, b, c, d) => combine([a, b, c, d]))),
            5 => Validated.CombineAsync(
                v[0], v[1], v[2], v[3], v[4],
                combine is null ? null! : (Func<int, int, int, int, int, int>)((a, b, c, d, e) => combine([a, b, c, d, e]))),
            6 => Validated.CombineAsync(
                v[0], v[1], v[2], v[3], v[4], v[5],
                combine is null ? null! : (Func<int, int, int, int, int, int, int>)((a, b, c, d, e, f) => combine([a, b, c, d, e, f]))),
            7 => Validated.CombineAsync(
                v[0], v[1], v[2], v[3], v[4], v[5], v[6],
                combine is null ? null! : (Func<int, int, int, int, int, int, int, int>)((a, b, c, d, e, f, g) => combine([a, b, c, d, e, f, g]))),
            8 => Validated.CombineAsync(
                v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7],
                combine is null ? null! : (Func<int, int, int, int, int, int, int, int, int>)((a, b, c, d, e, f, g, h) => combine([a, b, c, d, e, f, g, h]))),
            _ => throw new ArgumentOutOfRangeException(nameof(v)),
        };

    private sealed class CountingContext : SynchronizationContext
    {
        private int _posts;

        public int Posts => Volatile.Read(ref _posts);

        public override void Post(SendOrPostCallback d, object? state)
        {
            Interlocked.Increment(ref _posts);
            base.Post(d, state);
        }
    }
}
