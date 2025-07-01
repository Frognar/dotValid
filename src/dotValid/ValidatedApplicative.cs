using System;
using System.Linq;

namespace DotValid;

/// <summary>
/// The <c>ValidatedExtensions</c> class provides extension methods for enabling
/// functional programming constructs on validated values.
/// </summary>
public static class ValidatedApplicative
{
    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function.</typeparam>
    /// <typeparam name="T16">The type of the parameter parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, TResult>> Apply<T1, T2, T3, T4, T5, T6, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, TResult>> Apply<T1, T2, T3, T4, T5, TResult>(
        this Func<T1, T2, T3, T4, T5, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, TResult>> Apply<T1, T2, T3, T4, TResult>(
        this Func<T1, T2, T3, T4, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, TResult>> Apply<T1, T2, T3, TResult>(
        this Func<T1, T2, T3, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, TResult>> Apply<T1, T2, TResult>(
        this Func<T1, T2, TResult> map,
        Validated<T1> value) =>
        Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a function resulting in a new validated function with fewer parameters.
    /// </summary>
    /// <param name="map">The original function to which the value will be applied.</param>
    /// <param name="value">A Validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the returned value of the function.</typeparam>
    /// <returns>
    /// A Validated instance containing a function with one less parameter, or containing validation errors if the operation fails.
    /// </returns>
    public static Validated<TResult> Apply<T1, TResult>(
        this Func<T1, TResult> map,
        Validated<T1> value)
        => Validation.Succeed(map).Apply(value);

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, T9, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, T9, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">The validated function to which the value will be applied.</param>
    /// <param name="value">The validated instance containing the value to apply to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the resulting value of the function.</typeparam>
    /// <returns>
    /// A validated function with one fewer parameter if the value is valid, or a validated instance containing errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, T8, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => f(t1, t2, t3, t4, t5, t6, t7, t8))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, T7, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, T7, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, T7, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => f(t1, t2, t3, t4, t5, t6, t7))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, T6, TResult>> Apply<T1, T2, T3, T4, T5, T6, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, T6, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, T6, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, T6, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => f(t1, t2, t3, t4, t5, t6))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, T5, TResult>> Apply<T1, T2, T3, T4, T5, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, T5, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, T5, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, T5, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4, T5 t5) => f(t1, t2, t3, t4, t5))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, T4, TResult>> Apply<T1, T2, T3, T4, TResult>(
        this Validated<Func<T1, T2, T3, T4, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, T4, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, T4, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, T4, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3, T4 t4) => f(t1, t2, t3, t4))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, T3, TResult>> Apply<T1, T2, T3, TResult>(
        this Validated<Func<T1, T2, T3, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, T3, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, T3, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, T3, TResult>>,
                value: t1 => Validation.Succeed((T2 t2, T3 t3) => f(t1, t2, t3))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<Func<T2, TResult>> Apply<T1, T2, TResult>(
        this Validated<Func<T1, T2, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<Func<T2, TResult>>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<Func<T2, TResult>>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<Func<T2, TResult>>,
                value: t1 => Validation.Succeed((T2 t2) => f(t1, t2))));

    /// <summary>
    /// Applies a validated value to a validated function, producing a new validated function with one fewer parameter.
    /// </summary>
    /// <param name="map">A validated function to which the validated value will be applied.</param>
    /// <param name="value">A validated instance containing the value to be applied to the function.</param>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the function.</typeparam>
    /// <returns>
    /// A validated instance containing a function with one less parameter, or a collection of validation errors if the operation fails.
    /// </returns>
    public static Validated<TResult> Apply<T1, TResult>(
        this Validated<Func<T1, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Validation.Failed<TResult>(errors.Concat(otherErrors)),
                value: _ => Validation.Failed<TResult>(errors)),
            value: f => value.Match(
                errors: Validation.Failed<TResult>,
                value: t1 => Validation.Succeed(f(t1))));
}
