using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// V10: the asynchronous members agree with their synchronous counterparts, with dotResult's R8 semantics. Arguments
/// and the uninitialized default are checked immediately; whatever a delegate does, including throwing before it
/// returns a task, comes out when the result is awaited; cancellation for a requested token gives a canceled result;
/// and the case that passes through is already complete.
/// </summary>
public sealed class AsyncProperties : IDisposable
{
    private readonly CancellationTokenSource _source = new();

    public AsyncProperties() => _source.Cancel();

    private CancellationToken Canceled => _source.Token;

    public void Dispose() => _source.Dispose();

    [Property]
    public async Task MatchAsync_agrees_with_Match(Validated<int, string> validated, bool yields)
    {
        var expected = validated.Match(value => $"valid {value}", errors => $"invalid {errors.Length}");

        Assert.Equal(
            expected,
            await validated.MatchAsync(value => Deferred.Lift($"valid {value}", yields), errors => Deferred.Lift($"invalid {errors.Length}", yields)));
        Assert.Equal(
            expected,
            await validated.MatchAsync(value => Deferred.Lift($"valid {value}", yields), errors => $"invalid {errors.Length}"));
    }

    [Property]
    public async Task Exactly_one_branch_of_MatchAsync_runs_exactly_once(Validated<int, string> validated, bool yields)
    {
        var validCalls = 0;
        var invalidCalls = 0;

        _ = await validated.MatchAsync(_ => Deferred.Lift(++validCalls, yields), _ => Deferred.Lift(++invalidCalls, yields));
        _ = await validated.MatchAsync(_ => Deferred.Lift(++validCalls, yields), _ => ++invalidCalls);

        Assert.Equal(validated is Valid<int> ? 2 : 0, validCalls);
        Assert.Equal(validated is Invalid<string> ? 2 : 0, invalidCalls);
    }

    [Property]
    public async Task MapAsync_and_ThenAsync_agree_with_Map_and_Then(
        Validated<int, string> validated,
        Func<int, string> map,
        Func<int, Validated<bool, string>> next,
        bool yields)
    {
        Assert.Equal(validated.Map(map), await validated.MapAsync(value => Deferred.Lift(map(value), yields)));
        Assert.Equal(validated.Then(next), await validated.ThenAsync(value => Deferred.Lift(next(value), yields)));
    }

    [Property]
    public void An_invalid_validation_passes_through_at_once_and_nothing_runs(NonEmptyArray<int> errors)
    {
        var calls = 0;
        var invalidated = Validations.Invalid<string, int>(errors.Get);

        var mapped = invalidated.MapAsync(text => Task.FromResult(++calls));
        var continued = invalidated.ThenAsync(text => Task.FromResult(Validations.Valid<int, int>(++calls)));
        var matched = invalidated.MatchAsync(_ => Task.FromResult(++calls), all => all.Length);

        Assert.True(mapped.IsCompletedSuccessfully && continued.IsCompletedSuccessfully && matched.IsCompletedSuccessfully);
        ValidatedAssert.IsInvalid(errors.Get, mapped.Result);
        ValidatedAssert.IsInvalid(errors.Get, continued.Result);
        Assert.Equal(errors.Get.Length, matched.Result);
        Assert.Equal(0, calls);
    }

    [Property]
    public async Task What_a_delegate_throws_comes_out_only_when_awaited(Validated<int, string> validated)
    {
        var thrown = new InvalidOperationException("thrown by the delegate");
        Func<int, Task<int>> throwingValid = _ => throw thrown;
        Func<ImmutableArray<string>, Task<int>> throwingInvalid = _ => throw thrown;

        await AssertFailsOnlyWhenAwaited(thrown, () => validated.MatchAsync(throwingValid, throwingInvalid));
        if (validated is Valid<int>)
        {
            await AssertFailsOnlyWhenAwaited(thrown, () => validated.MapAsync(throwingValid));
            await AssertFailsOnlyWhenAwaited(thrown, () => validated.ThenAsync<int>(_ => throw thrown));
        }
    }

    [Property]
    public async Task A_throwing_synchronous_invalid_branch_comes_out_only_when_awaited(NonEmptyArray<int> errors)
    {
        var thrown = new InvalidOperationException("thrown by the invalid branch");

        await AssertFailsOnlyWhenAwaited(
            thrown,
            () => Validations.Invalid<string, int>(errors.Get).MatchAsync(_ => Task.FromResult(0), (Func<ImmutableArray<int>, int>)(_ => throw thrown)));
    }

    [Fact]
    public async Task The_library_never_resumes_on_the_callers_synchronization_context()
    {
        var context = new CountingContext();
        var validated = Validations.Valid<int, string>(1);
        var map = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var next = new TaskCompletionSource<Validated<int, string>>(TaskCreationOptions.RunContinuationsAsynchronously);
        ValueTask<Validated<int, string>> mapped;
        ValueTask<Validated<int, string>> continued;

        var previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(context);
        try
        {
            mapped = validated.MapAsync(_ => map.Task);
            continued = validated.ThenAsync(_ => next.Task);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
        }

        map.SetResult(2);
        next.SetResult(Validations.Valid<int, string>(3));

        Assert.Equal(Validations.Valid<int, string>(2), await mapped);
        Assert.Equal(Validations.Valid<int, string>(3), await continued);
        Assert.Equal(0, context.Posts);
    }

    [Property]
    public async Task A_null_or_uninitialized_result_of_a_step_fails_when_awaited(int value)
    {
        var validated = Validations.Valid<int, string>(value);

        var mapped = validated.MapAsync(_ => Task.FromResult((string)null!));
        var continued = validated.ThenAsync(_ => Task.FromResult(default(Validated<int, string>)));

        Assert.StartsWith("'map' returned null", (await Assert.ThrowsAsync<InvalidOperationException>(async () => await mapped)).Message, StringComparison.Ordinal);
        Assert.StartsWith("'next' returned an uninitialized", (await Assert.ThrowsAsync<InvalidOperationException>(async () => await continued)).Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Cancellation_for_a_requested_token_gives_a_canceled_result()
    {
        var validated = Validations.Valid<int, string>(1);
        var invalidated = Validations.Invalid<int, string>("boom");
        Func<int, Task<int>> throws = _ =>
        {
            Canceled.ThrowIfCancellationRequested();
            return Task.FromResult(0);
        };
        Func<int, Task<int>> canceled = _ => Task.FromCanceled<int>(Canceled);
        Func<ImmutableArray<string>, int> throwsSync = _ =>
        {
            Canceled.ThrowIfCancellationRequested();
            return 0;
        };

        await AssertCanceled(validated.MatchAsync(throws, _ => 0));
        await AssertCanceled(validated.MatchAsync(canceled, _ => Task.FromResult(0)));
        await AssertCanceled(invalidated.MatchAsync(throws, throwsSync));
        await AssertCanceled(invalidated.MatchAsync(throws, _ => Task.FromCanceled<int>(Canceled)));
        await AssertCanceled(validated.MapAsync(throws));
        await AssertCanceled(validated.MapAsync(canceled));
        await AssertCanceled(validated.ThenAsync(_ => Task.FromCanceled<Validated<int, string>>(Canceled)));
    }

    [Property]
    public void Missing_delegates_are_rejected_immediately_whatever_the_case(Validated<int, string> validated)
    {
        Assert.Equal("valid", Assert.Throws<ArgumentNullException>(() => validated.MatchAsync((Func<int, Task<int>>)null!, (Func<ImmutableArray<string>, Task<int>>)(_ => Task.FromResult(0)))).ParamName);
        Assert.Equal("invalid", Assert.Throws<ArgumentNullException>(() => validated.MatchAsync(_ => Task.FromResult(0), (Func<ImmutableArray<string>, Task<int>>)null!)).ParamName);
        Assert.Equal("valid", Assert.Throws<ArgumentNullException>(() => validated.MatchAsync(null!, _ => 0)).ParamName);
        Assert.Equal("invalid", Assert.Throws<ArgumentNullException>(() => validated.MatchAsync(_ => Task.FromResult(0), (Func<ImmutableArray<string>, int>)null!)).ParamName);
        Assert.Equal("map", Assert.Throws<ArgumentNullException>(() => validated.MapAsync((Func<int, Task<int>>)null!)).ParamName);
        Assert.Equal("next", Assert.Throws<ArgumentNullException>(() => validated.ThenAsync((Func<int, Task<Validated<int, string>>>)null!)).ParamName);
    }

    // Counts continuations posted back to it; ConfigureAwait(false) in the library means none.
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

    private static async Task AssertFailsOnlyWhenAwaited<TResult>(Exception expected, Func<ValueTask<TResult>> call)
    {
        ValueTask<TResult> pending = default;

        var thrownByTheCall = Record.Exception(() => { pending = call(); });

        Assert.Null(thrownByTheCall);
        Assert.Same(expected, await Assert.ThrowsAnyAsync<Exception>(async () => await pending));
    }

    private static async Task AssertCanceled<TResult>(ValueTask<TResult> pending)
    {
        var task = pending.AsTask();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        Assert.True(task.IsCanceled, "Expected the result to be canceled, not faulted or successful.");
    }
}
