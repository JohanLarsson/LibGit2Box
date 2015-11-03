namespace LibGit2Box
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;

    using LibGit2Sharp;

    public static class Git
    {
        private static readonly CommitOptions CommitOptions = new CommitOptions { AllowEmptyCommit = false };

        public static ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

        public static bool IsValid(DirectoryInfo directory)
        {
            return LibGit2Sharp.Repository.IsValid(directory.FullName);
        }

        public static void InitRepository(DirectoryInfo directory)
        {
            if (!LibGit2Sharp.Repository.IsValid(directory.FullName))
            {
                var init = LibGit2Sharp.Repository.Init(directory.FullName);
                Log.Add(init);
            }
        }

        public static void Stage(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                repository.Stage(file.FullName);
                Log.Add($"Staged: {file.Name}");
            }
        }

        public static void Commit(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                try
                {
                    var status = repository.RetrieveStatus(file.FullName);

                    switch (status)
                    {
                        case FileStatus.Unaltered:
                            Log.Add($"Commit, FileStatus: {status}");
                            return;
                        case FileStatus.Nonexistent:
                        case FileStatus.Added:
                        case FileStatus.Staged:
                        case FileStatus.Removed:
                        case FileStatus.RenamedInIndex:
                        case FileStatus.StagedTypeChange:
                        case FileStatus.Untracked:
                        case FileStatus.Modified:
                        case FileStatus.Missing:
                        case FileStatus.TypeChanged:
                        case FileStatus.RenamedInWorkDir:
                            {
                                var commit = repository.Commit("Update", CommitOptions);
                                Log.Add($"Commit: {commit.Message}");
                                break;
                            }
                        case FileStatus.Unreadable:
                        case FileStatus.Ignored:
                            throw new InvalidOperationException($"FileStatus: {status}");
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception e)
                {
                    Log.Add(e.Message);
                }
            }
        }

        public static void Revert(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                repository.CheckoutPaths("master", new[] { file.FullName }, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
                Log.Add($"Revert: {file.Name}");
            }
        }
    }
}