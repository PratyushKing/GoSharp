using System;
namespace gosharp;

public class stdLib {
    
    public stdLib() { }

    public void RunFuncLine(string ln) {

    }

    public virtual object? _runFuncFromStruct(LibCode code) {
        return null;
    }
}

public class LibCode {
    public string FuncName;
    public List<string> Args = new();

    public LibCode(string ln) {
        if (ln.Split(' ').Length < 1) { return; } // invalid
        
        // CASE 1: line has no args.
        if (ln.Split(' ').Length == 1) {
            Args = new();
            FuncName = ln.Split(' ')[0];
            return;
        }

        // CASE 2: line has args.
        if (ln.Split(' ').Length > 1) {
            FuncName = ln.Split(' ')[0];
            var lnArgs = ln.Split(' ').ToList();
            lnArgs.Remove(FuncName);
            for (var i = 0; i < lnArgs.Count - 1; i++) {
                Args.Add(lnArgs[i]);
            }
            return;
        }
    }
}