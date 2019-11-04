using System;
using System.IO;

namespace DelegateDemo
{
    
    // Delegate Specification
    public class MyClass
    {
        // Define a delegate named LogHandler, which will encapsulate
        // any method that takes a string as the parameter and returns no value
        public delegate void LogHandler(string message);

        // Define an Event based on the above Delegate
        public event LogHandler Log;

        // Instead of having the Process() function take a delegate
        // as a parameter, we've declared a Log event. Call the Event,
        // using the OnXXXX Method, where XXXX is the name of the Event.
        public void Process()
        {
            OnLog("Process() begin");
            OnLog("Process() end");
        }
        // By Default, create an OnXXXX Method, to call the Event
        protected void OnLog(string message)
        {
            if (Log != null)
            {
                Log(message);
            }
        }

    }

    // The FileLogger class merely encapsulates the file I/O
    public class FileLogger
    {
        FileStream fileStream;
        StreamWriter streamWriter;

        // Constructor
        public FileLogger(string filename)
        {
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
        }

        // Member Function which is used in the Delegate
        public void Logger(string s)
        {
            streamWriter.WriteLine($"{s} - {DateTime.Now}");
        }

        public void Close()
        {
            streamWriter.Close();
            fileStream.Close();
        }
    }

    class Program
    {
        // Static Function: To which is used in the Delegate. To call the Process()
        // function, we need to declare a logging function: Logger() that matches
        // the signature of the delegate.
        static void Logger(string s)
        {
            Console.WriteLine(s);
        }

        static void Main(string[] args)
        {
            //------------------
            // write to console
            //------------------
            //MyClass myClass = new MyClass();
            //// Crate an instance of the delegate, pointing to the logging function.
            //// This delegate will then be passed to the Process() function.
            //MyClass.LogHandler myLogger = new MyClass.LogHandler(Logger);
            //myClass.Process(myLogger);

            //------------------
            // write to file
            //------------------
            //FileLogger fl = new FileLogger("process.log");
            //MyClass myClass = new MyClass();
            //// Crate an instance of the delegate, pointing to the Logger()
            //// function on the fl instance of a FileLogger.
            //MyClass.LogHandler myLogger = new MyClass.LogHandler(fl.Logger);
            //myClass.Process(myLogger);
            //fl.Close();

            //------------------
            // multiple delegate - Multicasting
            //------------------
            //FileLogger fl = new FileLogger("process.log");
            //MyClass myClass = new MyClass();
            //// Crate an instance of the delegates, pointing to the static
            //// Logger() function defined in the TestApplication class and
            //// then to member function on the fl instance of a FileLogger.
            //MyClass.LogHandler myLogger = null;
            //myLogger += new MyClass.LogHandler(Logger);
            //myLogger += new MyClass.LogHandler(fl.Logger);
            //myClass.Process(myLogger);
            //fl.Close();

            FileLogger fl = new FileLogger("process.log");
            MyClass myClass = new MyClass();
            // Subscribe the Functions Logger and fl.Logger
            myClass.Log += new MyClass.LogHandler(Logger);
            myClass.Log += new MyClass.LogHandler(fl.Logger);
            // The Event will now be triggered in the Process() Method
            myClass.Process();
            fl.Close();
        }
    }
}
