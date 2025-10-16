
using System;
using System.Collections.Generic;	
using JoystickCompiler;


public class Parser {
	public const int _EOF = 0;
	public const int _MOVE = 1;
	public const int _BUTTON = 2;
	public const int _PRESS = 3;
	public const int _RELEASE = 4;
	public const int _WAIT = 5;
	public const int _DIRECTION = 6;
	public const int _BUTTON_NAME = 7;
	public const int _NUMBER = 8;
	public const int maxT = 9;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

    private JoystickCompiler.JoystickCompiler compiler;

    // Agrega este método para establecer la referencia al compilador
    public void SetCompiler(JoystickCompiler.JoystickCompiler compiler)
    {
        this.compiler = compiler;
    }

    public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void JoystickLang() {
		while (la.kind == 1 || la.kind == 2 || la.kind == 5) {
			Command();
		}
		Expect(0);
	}

	void Command() {
		if (la.kind == 1) {
			MovementCommand();
		} else if (la.kind == 2) {
			ButtonCommand();
		} else if (la.kind == 5) {
			WaitCommand();
		} else SynErr(10);
	}

	void MovementCommand() {
		Expect(1);
		Expect(6);
		Console.WriteLine("Moviendo joystick: " + t.val);
        compiler.ExecuteMovement(t.val);

    }



    void ButtonCommand()
    {
        Expect(2); // BUTTON
        Expect(7); // BUTTON_NAME
        string button = t.val; // Guardar el valor del botón

        if (la.kind == 3)
        {
            Get();
        }
        else if (la.kind == 4)
        {
            Get();
        }
        else SynErr(11);

        string action = t.val; // Guardar el valor de la acción

        Console.WriteLine("Boton " + button + ": " + action);
        compiler.ExecuteButton(button, action);
    }

 

    void WaitCommand() {
		Expect(5);
		Expect(8);
		int seconds = int.Parse(t.val);
		Console.WriteLine("Esperando " + seconds + " segundos...");
		System.Threading.Thread.Sleep(seconds * 1000);
		
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		JoystickLang();
		Expect(0);

	}

 

    static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "MOVE expected"; break;
			case 2: s = "BUTTON expected"; break;
			case 3: s = "PRESS expected"; break;
			case 4: s = "RELEASE expected"; break;
			case 5: s = "WAIT expected"; break;
			case 6: s = "DIRECTION expected"; break;
			case 7: s = "BUTTON_NAME expected"; break;
			case 8: s = "NUMBER expected"; break;
			case 9: s = "??? expected"; break;
			case 10: s = "invalid Command"; break;
			case 11: s = "invalid ButtonCommand"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
