﻿namespace LibGit2Box
{
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using JetBrains.Annotations;
    public class ViewModel : INotifyPropertyChanged
    {
        private static readonly FileInfo DataFile = new FileInfo(@"C:\Temp\LibGit2Box\Data.txt");

        private string _text;

        public ViewModel()
        {
            if (!Directory.Exists(DataFile.DirectoryName))
            {
                Directory.CreateDirectory(DataFile.DirectoryName);
            }
            if (File.Exists(DataFile.FullName))
            {
                Text = File.ReadAllText(DataFile.FullName);
            }

            SaveCommand = new RelayCommand(_ => File.WriteAllText(DataFile.FullName, Text));
            InitRepositoryCommand = new RelayCommand(_ => Git.InitRepository(DataFile.Directory), _ => !Git.IsValid(DataFile.Directory));
            StageCommand = new RelayCommand(_ => Git.Stage(DataFile));
            CommitCommand = new RelayCommand(_ => Git.Commit(DataFile));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == _text)
                {
                    return;
                }
                _text = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand InitRepositoryCommand { get; }

        public ICommand StageCommand { get; }

        public ICommand CommitCommand { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
