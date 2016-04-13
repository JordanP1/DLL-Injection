using Controller.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller
{
    public partial class Controller : Form
    {
        //List to store found TargetApplications.
        private List<TargetApplication> _processList;
        //Scan for newly opening and closing TargetApplications.
        private ProcessWatcher _processWatcher;

        public Controller()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this._processList = new List<TargetApplication>();
            this._processWatcher = new ProcessWatcher("TargetApplication");
            this._processWatcher.NewProcess += _processWatcher_NewProcess;
            this._processWatcher.ClosedProcess += _processWatcher_ClosedProcess;
            this._processWatcher.Start();
        }

        //Grabs the currently selected process of TargetApplication in the listbox.
        public TargetApplication TargetApplication
        {
            get
            {
                TargetApplication targetApplication = this.listBoxProcessList.SelectedItem as TargetApplication;
                if (targetApplication != null)
                {
                    return targetApplication;
                }

                return null;
            }
        }

        //In the case of any unhandled exception errors.
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Cleanup();
        }

        //Called when a new process instance of TargetApplication is found.
        //Adds it to the process list and listbox.
        private void _processWatcher_NewProcess(Process process)
        {
            TargetApplication tap = new TargetApplication(process);
            this._processList.Add(tap);
            this.listBoxProcessList.Items.Add(tap);

            //Select the first process by default.
            if (this.listBoxProcessList.Items.Count == 1)
            {
                this.listBoxProcessList.SelectedIndex = 0;
            }
        }

        //Called when an existing process in the process list has closed.
        //Removes it from the process list and listbox.
        //Also deinitializes the TargetApplication instance.
        private void _processWatcher_ClosedProcess(Process process)
        {
            for (int i = 0; i < this._processList.Count; i++)
            {
                TargetApplication tap = this._processList[i];
                if (tap.Process.Id == process.Id)
                {
                    this._processList.RemoveAt(i);
                    this.listBoxProcessList.Items.RemoveAt(i);
                    i--;
                    tap.Deinitialize(false);

                    //Select another process, if available.
                    //Prioritizes process above closing process,
                    //if available.
                    if (this.listBoxProcessList.Items.Count != 0)
                    {
                        int newIndex = i - 1;
                        int index = i < 0 ? 0 : i;
                        this.listBoxProcessList.SelectedIndex = index;
                    }
                    break;
                }
            }
        }

        //Deinitialize all TargetApplication instances and calls anything
        //else that may need to be called before closing the application.
        private void Cleanup()
        {
            this._processWatcher.Stop();

            for (int i = 0; i < this._processList.Count; i++)
            {
                this._processList[i].Deinitialize();
                this._processList.RemoveAt(i);
                i--;
            }
        }

        //Initialize the TargetApplication instance that gets selected.
        private void listBoxProcessList_SelectedIndexChanged(object sender, EventArgs e)
        {
            TargetApplication targetApplication = this.TargetApplication;
            if (targetApplication != null)
            {
                targetApplication.Initialize();
                targetApplication.IncreaseIndex = (int)this.numericUpDownIncreaseIndex.Value;
            }
        }

        //Call the Cleanup() function to handle anything that needs to be
        //handled before closing the application.
        private void Controller_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Cleanup();
        }

        private void buttonSendString_Click(object sender, EventArgs e)
        {
            this.SendInput();
        }

        //Send the text from the textBoxPrintString control to the DLL,
        //which calls the printf() function to print the text into the
        //TargetApplication's console.
        private void SendInput()
        {
            if (!string.IsNullOrWhiteSpace(this.textBoxPrintString.Text))
            {
                TargetApplication targetApplication = this.TargetApplication;
                if (targetApplication != null)
                {
                    targetApplication.PrintString(this.textBoxPrintString.Text);

                    //Clear the textbox after sending.
                    this.textBoxPrintString.Clear();
                }
            }
        }

        //Trigger the send method from pressing enter in the textBoxPrintString.
        private void textBoxPrintString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Suppress to prevent the chime sound.
                e.SuppressKeyPress = true;
                this.SendInput();
            }
        }

        //Trigger the IncreaseIndex() method through our DLL, which triggers
        //the method within our selected TargetApplication process.
        private void buttonTriggerIncrease_Click(object sender, EventArgs e)
        {
            TargetApplication targetApplication = this.TargetApplication;
            if (targetApplication != null)
            {
                targetApplication.TriggerIncreaseIndex();
            }
        }

        //Change the amount the index is increased with each trigger.
        //This applies to both the force-trigger via the Trigger button, and
        //also TargetApplication's increase function from pressing enter.
        private void numericUpDownIncreaseIndex_ValueChanged(object sender, EventArgs e)
        {
            TargetApplication targetApplication = this.TargetApplication;
            if (targetApplication != null)
            {
                targetApplication.IncreaseIndex = (int)this.numericUpDownIncreaseIndex.Value;
            }
        }
    }
}
