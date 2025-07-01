# dotValid - Functional Validation for C#

[![.net workflow](https://github.com/Frognar/dotValid/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Frognar/dotValid/actions/workflows/dotnet.yml)

dotValid is a small, focused C# library designed to make your validation logic more robust and user-friendly by embracing a functional approach. Unlike traditional validation methods that often stop at the first error, dotValid allows you to accumulate all validation errors, providing a complete picture of what went wrong.

## Why dotValid?
Imagine a user filling out a form with multiple fields. If you validate each field sequentially and stop on the first error, the user has to correct one error, resubmit, only to find another. With dotValid, you can validate all fields and present every error at once, leading to a much smoother user experience.

This library is particularly useful for:
- Validating incoming API requests.
- Form validation in desktop or web applications.
- Any scenario where you want to provide comprehensive error feedback.

# Give it a star ⭐ !

If you find this project valuable, please consider giving it a star! Your support helps others discover this work and encourages further development.

---

### Quick Start

This section provides a quick overview of how to use `DotValid` to handle validation results in a functional and robust way.

#### 1. Defining a `Validated<T>` Result

The core of `DotValid` is the `Validated<T>` record struct, which explicitly represents either a successful value or a collection of validation errors.

```csharp
using DotValid;
using System.Collections.Generic;

// Example: A simple object to validate
public record User(string Name, int Age);

// --- Creating Success Results ---

// Using the static Validation helper
Validated<User> validUser = Validation.Succeed(new User("John Doe", 30));

// --- Creating Failure Results ---

// Using the static Validation helper with a single error
Validated<User> invalidUserName = Validation.Failed<User>(
    [new ValidationError("Name", "Name cannot be empty.")]);

// Using the static Validation helper with multiple errors
Validated<User> invalidUserMultipleErrors = Validation.Failed<User>(
    [
        new ValidationError("Name", "Name cannot be empty."),
        new ValidationError("Age", "Age must be greater than 0.")
    ]);
```

#### 2. Checking Validation State and Accessing Values/Errors

Yu can easily check if a `Validated<T>` instance is valid and access its content using `IsValid` or the `Match` method.
```csharp
using DotValid;
using System;
using System.Collections.Generic;
using System.Linq;

// Assume we have a validated user result
Validated<User> result = Validation.Succeed(new User("Alice", 25)); // Or a failed result

// Check if valid
if (result.IsValid)
{
    Console.WriteLine("Validation Succeeded!");
    // You can access the value directly in a conditional block (though Match is preferred for safety)
    string str = result.Match(
        errors: _ => "", // This path won't be taken for an invalid result
        value: user => $"User: {user.Name}, Age: {user.Age}"
    );

    Console.WriteLine(str);
}
else
{
    Console.WriteLine("Validation Failed!");
    // Match is ideal for handling both success and failure paths
    string str = result.Match(
        errors: errors => string.Join(Environment.NewLine, errors.Select(error => $"- Property: {error.PropertyName}, Error: {error.Error}")),
        value: _ => "" // This path won't be taken for an invalid result
    );

    Console.WriteLine(str);
}

// More concise error handling with Match
string matched = result.Match(
    value: user => $"Successfully processed user: {user.Name}",
    errors: errors => $"Validation errors: {string.Join(", ", errors.Select(e => $"{e.PropertyName}: {e.Error}"))}"
);

Console.WriteLine(matched);
```

#### 3. Checking Validation State and Accessing Values/Errors
The true power of `DotValid` comes with its applicative `Apply` extension methods, allowing you to combine multiple independent `Validated<T>` results and accumulate all errors, rather than stopping at the first failure.
```csharp
using DotValid;
using System;
using System.Collections.Generic;
using System.Linq;

public record Person(string FirstName, string LastName, int Age);

// Individual validated inputs
Validated<string> firstNameValidated = Validation.Succeed("Jane");
Validated<string> lastNameValidated = Validation.Failed<string>(new[] { new ValidationError("LastName", "Last name cannot be empty.") });
Validated<int> ageValidated = Validation.Failed<int>(new[] { new ValidationError("Age", "Age must be positive.") });
Validated<string> emailValidated = Validation.Succeed("jane.doe@example.com"); // Another valid input

// Define a constructor function for Person
Func<string, string, int, Person> createPerson =
    (firstName, lastName, age) => new Person(firstName, lastName, age);

// Use Apply to combine the validated inputs
Validated<Person> validatedPerson = createPerson
    .Apply(firstNameValidated)  // Apply the first validated value
    .Apply(lastNameValidated)   // Apply the second validated value
    .Apply(ageValidated);      // Apply the third validated value
    // .Apply(emailValidated); // You could continue applying more if Person had an email field

// Handle the final result
string result = validatedPerson.Match(
    value: person => $"Created valid person: {person.FirstName} {person.LastName}, Age: {person.Age}",
    errors: errors => $"Failed to create person. Errors:{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(error => $"- {error.PropertyName}: {error.Error}"))}"
);

Console.WriteLine(result);

// Expected Output for the example above:
// Failed to create person. Errors:
// - LastName: Last name cannot be empty.
// - Age: Age must be positive.
```
This `Apply` pattern is extremely useful for form validations or any scenario where you want to show the user all validation issues at once, rather than just the first one.

# Contribution

If you would like to contribute to this project, check out [CONTRIBUTING](https://github.com/Frognar/dotResult/blob/main/CONTRIBUTING.md) file.

# License

This project is licensed under the terms of the [MIT](https://github.com/Frognar/dotResult/blob/main/LICENSE) license.
