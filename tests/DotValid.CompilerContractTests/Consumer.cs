namespace DotValid.CompilerContractTests;

/// <summary>
/// Wraps members in a consumer file that imports DotValid the documented way.
/// </summary>
internal static class Consumer
{
    public static string File(string members, string inNamespace = "Consumer") => $$"""
          using System;
          using System.Threading.Tasks;
          using DotValid;
          using static DotValid.Prelude;

          namespace {{inNamespace}};

          public static class Sample
          {
          {{members}}
          }
          """;
}
