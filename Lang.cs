using System;
namespace gosharp;

public static class Lang {
    public static readonly List<string> standardLibs = new() {
        "fmt",
        "math"
    };

    public static readonly List<string> reservedKeywords = new() {
        "break",
        "case",
        "chan",
        "const",
        "continue",
        "default",
        "defer",
        "else",
        "fallthrough",
        "for",
        "func",
        "go",
        "goto",
        "if",
        "import",
        "interface",
        "map",
        "package",
        "range",
        "return",
        "select",
        "struct",
        "switch",
        "type",
        "var"
    };

    public static List<GoInstructions>? GoCodeToInstructions(string goCode, bool verbose = true) {
        List<GoInstructions> generatedCode = new();
        List<string> currentLoadedPackages = new();

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
        if (toUseCode[toUseCode.Count - 1] == "") {
            toUseCode.RemoveAt(toUseCode.Count - 1);
        }
        toUseCode.Add("EOF"); // security reasons

        if (verbose) darkWrite(string.Join('\n', toUseCode));

        if (toUseCode[0].StartsWith("package")) {
            if (toUseCode[0] == "package ") { genErr("Expected 'package_name', found '" + toUseCode[1].Split(' ')[0] + "'", 0, ("package ").Length); return null; }
            if (toUseCode[0].Split(' ').Length != 2) { genErr("Expected semicolon or newline", 1, ("package " + toUseCode[0].Split(' ')[1]).Length); return null; }
            if (!AreAllLettersOrNumbersOnly(toUseCode[0].Split(' ')[1])) { genErr("Invalid character found in the package name.", 1, ("package ").Length); return null; }
            programName = toUseCode[0].Split(' ')[1];
        } else {
            genErr("Expected 'package', found '" + toUseCode[0].Split(' ')[0] + "'", 1, 0);
        }

        int cLine = 1;
        bool ifInsideFunc = false;
        string currentFuncName = "";
        foreach (var ln in toUseCode) {

            if (ln.StartsWith("import")) {
                string cLn = ln.Replace(" ", "");
                string toAddPkg = "";
                if (cLn.StartsWith("import\"")) {
                    for (var i = ("import\"").Length; i < cLn.Length; i++) {
                        if (cLn[i] == '"') {
                            if (i < cLn.Length - 1) {
                                genErr("Unexpected set of characters after import declaration", cLine, i); // i+1 due to the next char rather than the current end
                                return null;
                            }
                            break;
                        }
                        else toAddPkg += cLn[i];
                    }
                }
                if (verbose) darkWrite("Adding Package: '" + toAddPkg + "'");

                bool added = false;
                foreach (var availablePkg in standardLibs) {
                    if (toAddPkg == availablePkg) {
                        currentLoadedPackages.Add(toAddPkg);
                        if (verbose) darkWrite("Package successfully added! ('" + toAddPkg + "')");
                        added = true;
                        break;
                    }
                }
                if (!added) genErr("Package '" + toAddPkg + "' is not found in standard packages", cLine, ("import \"").Length);
            }

            if (ln.StartsWith("func")) {
                if (ifInsideFunc) { genErr("Functions in gosharp cannot be used as objects if created inside a function.", cLine, 0); return null; }
                if (ln.Split(' ').Length < 2) {
                    genErr("Expected 'func_name'", cLine, ("func ").Length);
                }
                if (ln.StartsWith("func main")) {
                    // bypass args passing since not required
                    ifInsideFunc = true;
                    currentFuncName = "main";
                    continue;
                }
                string funcName = "";
                string cWord = "";
                bool funcFound = false;
                bool funcNameFound = false;
                bool argsFound = false;
                bool inArgs = false;
                foreach (var ch in ln) {
                    cWord += ch;
                    if (cWord.StartsWith("func")) {
                        if (funcFound) {
                            genErr("Function name cannot be 'func'", cLine, ("func ").Length);
                        }
                        funcFound = true;
                        cWord = "";
                        continue;
                    }
                    if (funcFound && !funcNameFound && (cWord.EndsWith(' ') || cWord.EndsWith('('))) {
                        if (cWord.StartsWith(' ')) cWord.TrimStart(' ');
                        if (IsReservedKeyword(cWord)) {
                            genErr("Function name cannot be '" + cWord + "' (reserved_keyword)", cLine, ("func ").Length);
                            return null;
                        }
                        funcNameFound = true;
                        funcName = cWord;
                        cWord = "";
                        continue;
                    }
                    if (funcNameFound && funcFound && !argsFound && cWord == "(") {
                        inArgs = true;
                    }
                    // WORK IN PROGRESS, WILL ADD ARG PASSING SOON
                    if (funcNameFound && funcFound && inArgs) {
                        
                    }
                }
                ifInsideFunc = true;
                currentFuncName = funcName;
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

    public static void errWrite(string str) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(str);
        Console.ResetColor();
    }

    public static void genErr(string errReason, int ln, int ch) => errWrite("ERR: " + errReason + " [line: " + ln + ", char: " + ch + "]");

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

    public static bool IsReservedKeyword(string str) {
        foreach (var keyword in reservedKeywords) {
            if (keyword == str) {
                return true;
            }
        }
        return false;
    }
}

public class GoInstructions {
    public Task task;
    public string val;

    public GoInstructions(Task task, string value) {
        this.task = task;
        this.val = value;
    }
}

public enum Task {

}