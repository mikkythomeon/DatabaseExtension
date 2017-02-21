using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseExtension;

namespace DatabaseExtensionExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ExampleForm());





        }

        static void ExampleForm()
        {
            DatabaseExtension.DataApplication _databases;

            //create a new instance of a DataApplication
            //The DataApplication holds all the definitions of SQL queries and parameters in memory for use when needed later. 
            _databases = new DataApplication();
            _databases = new DatabaseExtension.DataApplication();
            //load the DataApplication object by deserialization (Check the Queries.xml file)
            System.IO.StreamReader streamreader = new System.IO.StreamReader("./Data/Queries.xml");
            System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(_databases.GetType());
            _databases = (DataApplication)deserializer.Deserialize(streamreader);
            streamreader.Close();
            _databases.Initialize();


            //retrieve  a database query and execute it
            DataApplicationQuery q = _databases.GetQuery("SetProductLocation");
            q.SetParameter("@PLine", "01");             //production line numnber
            q.SetParameter("@Product", "8120351402");   //product code
            q.SetParameter("@Serial", "0123456789");    //product serial number
            q.SetParameter("@Location", "23");          //product location
            q.ExecuteNonQuery();

        }

    }
}
