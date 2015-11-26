using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ClientTcpWorker worker; 
        
        #region Upload


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
     
        public void ExecuteUploadCommand(object parameter)
        {
            OpenFileDialog opd = new OpenFileDialog();

            if (DialogResult.OK == opd.ShowDialog())
            {
                worker = new ClientTcpWorker(5050, "::1");
                Task.Factory.StartNew(() => worker.SendFileDict(opd.FileName));
            }
        }
        #endregion

        #region Download

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            { _selectedIndex = value;
              OnPropertyChanged("SelectedIndex");
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
            worker = new ClientTcpWorker(5050, "::1");
            if (_selectedIndex != -1)
            {
                Task.Factory.StartNew(() => worker.SendIdDict(DataBaseInfo.Rows[_selectedIndex].ItemArray[0].ToString()));
            }
           
        }
        #endregion

        #region Refresh
        RelayCommand _refreshCommand;
        public ICommand Refresh
        {
            get
            {
                if (_refreshCommand == null)
                    _refreshCommand = new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);
                return _refreshCommand;
            }
        }

        private DataTable DataBaseInfo;

        public DataTable _dataTable 
        {
            get { return DataBaseInfo; } 
            set
            {
               DataBaseInfo = value;
               OnPropertyChanged("_dataTable");
            } 
        }

        public void ExecuteRefreshCommand(object parameter)
        {
          
            worker = new ClientTcpWorker(5050, "::1");
            try
            {
              _dataTable = Task.Factory.StartNew(worker.RecieveDatabaseTable).Result;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не удалось обновить таблицу " +
                                $"Error: {e.Message}");    
            }
        }
        #endregion

       

        public bool CanExecuteUploadCommand(object parameter)
        {
            return true;
        }

        public bool CanExecuteDownloadCommand(object parameter)
        {
            return true;
        }

        public bool CanExecuteRefreshCommand(object parameter)
        {
            return true;
        }
    }
}
