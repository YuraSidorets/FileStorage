using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{ 
    public class MainWindowViewModel : ViewModelBase
    {

        RelayCommand _uploadCommand;
        public ICommand Upload
        {
            get
            {
                if (_uploadCommand == null)
                    _uploadCommand = new RelayCommand(ExecuteUploadCommand, CanExecuteUploadCommand);
                return _uploadCommand;
            }
        }

      
        public delegate void Mthd(string text);

        public void ExecuteUploadCommand(object parameter)
        {
            OpenFileDialog opd = new OpenFileDialog();
           
            if (DialogResult.OK == opd.ShowDialog())
            {
                ClientTcpWorker rm = new ClientTcpWorker(5050, "::1");
                Mthd mthd = rm.SendFileDict;
                Paralel(mthd,opd.FileName);
            }
        }

        RelayCommand _downloadCommand;
        public ICommand Download
        {
            get
            {
                if (_downloadCommand == null)
                    _downloadCommand = new RelayCommand(ExecuteDownloadCommand, CanExecuteDownloadCommand);
                return _downloadCommand;
            }
        }

        public void ExecuteDownloadCommand(object parameter)
        {
            throw new NotImplementedException();

                //ClientTcpWorker rm = new ClientTcpWorker(5050, "::1");
                //Mthd mthd = rm.SendFileDict;
                //Paralel(mthd, Id из DataGrid'a );
           }

        private void Paralel(Delegate method, string param)
        {
            Task.Factory.StartNew(() => method.DynamicInvoke(param));
        }

        public bool CanExecuteUploadCommand(object parameter)
        {
            return true;
        }

        public bool CanExecuteDownloadCommand(object parameter)
        {
            return true;
        }
    }
}
