using System;

namespace gosharp;

public static class Program {
    public static void Main(string[] args) {
        Console.WriteLine(gosharp.Lang.GoCodeToInstructions(File.ReadAllText("sample.go")));
    }
}