using System;
namespace gosharp;

public class fmtLibImpl : stdLib {
    public fmtLibImpl() {

    }

    public override object? _runFuncFromStruct(LibCode code)
    {
        switch (code.FuncName) {
            case "Println": return "print ";
            default: return 1;
        }
    }
}