using System;
namespace gosharp;

public static class Lang {
    public const Dictionary<string, string> standardLibs = new() {
        {"fmt", "main I/O library"}
    };

    public static string GoCodeToInstructions(string goCode, bool verbose = true) {
        string generatedCode = "";

        string programName = "main";

        var toUseCode = goCode.Split('\n').ToList();
        
        for (var i = 0; i < toUseCode.Count - 1; i++) {
            if (toUseCode[i] == "" || string.IsNullOrWhiteSpace(toUseCode[i])) toUseCode.Remove(toUseCode[i]);
            while (toUseCode[i].StartsWith(" ")) {
                toUseCode[i] = toUseCode[i].TrimStart(' ');
            }
            while (toUseCode[i].StartsWith("\t")) {
                toUseCode[i] = toUseCode[i].TrimStart('\t');
            }
        }

        if (verbose) darkWrite(string.Join('\n', toUseCode));

        int cLine = 1;
        foreach (var ln in toUseCode) {

            if (ln.Split(' ')[0] == "package") {
                if (ln.Split(' ').Length != 2) return genErr("Invalid `package` syntax, name not present or name cannot have spaces", cLine, ("package ").Length);
                if (!AreAllLettersOrNumbersOnly(ln.Split(' ')[1])) return genErr("Invalid `package` syntax, name can only be letters or numbers", cLine, ("package ").Length);
                if (!AreAllLettersOnly(ln.Split(' ')[1])) writeWarn("It is recommended not to use numbers in package name", cLine, ("package ").Length);
                programName = ln.Split(' ')[1];
            }

            if (ln.StartsWith("import")) {

            }

            cLine++;
        }

        darkWrite("[Compilation completed!]");
        return generatedCode;
    }

    public static void darkWrite(string str) {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(str);
        Console.ResetColor();
    }

    public static void warnWrite(string str) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(str);
        Console.ResetColor();
    }

    public static string genErr(string errReason, int ln, int ch) { return "ERR: " + errReason + " [line: " + ln + ", char: " + ch + "]"; }

    public static void writeWarn(string warn, int ln, int ch) => warnWrite("WARN: " + warn + " [line: " + ln + ", char: " + ch + "]");

    public static bool AreAllLettersOnly(string str) {
        foreach (var ch in str) {
            if (!char.IsLetter(ch)) return false;
        }
        return true;
    }

    public static bool AreAllLettersOrNumbersOnly(string str) {
        foreach (var ch in str) {
            if (!char.IsLetter(ch) && !char.IsNumber(ch)) return false;
        }
        return true;
    }
}