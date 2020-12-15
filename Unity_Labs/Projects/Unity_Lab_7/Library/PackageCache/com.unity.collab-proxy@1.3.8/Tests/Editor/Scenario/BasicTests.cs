namespace Unity.Cloud.Collaborate.Tests
{
    public class BasicTests : ScenarioTestBase
    {
//        [UnityTest]
//        public IEnumerator AddedAssetsShowAsUntrackedChanges()
//        {
//            return atc.Run(async () =>
//            {
//
//                // ensure initial clean state.
//                await EnsureCleanChangesPageInitially();
//
//                // ensure file doesn't already exist.
//                const string filename = "file1.txt";
//                File.Exists(filename).ShouldBe(false, $"{filename} already exists");
//
//                // todo - wrap operations like these in a dedicated helper class.
//                File.WriteAllText(filename, "added file empty content .. ");
//
//                // todo - ahmad : port the state monitoring implementation from
//                // collab ver to here to avoid arbitrary UI wait times.
//                await Task.Delay(1000);
//
//                var entries = (await BackendProvider.Instance.GetChanges());
//                entries.Count.ShouldBe(1, "changes count did not add file1");
//                entries[0].Path.ShouldBe("file1.txt", "change added is not named file1.txt");
//                entries[0].Status.ShouldBe(ChangeEntryStatus.Untracked, "change added is untracked");
//
//                // clean up the file. (todo - ahmad) : this should be added in an after() method.
//                File.Delete(filename);
//            });
//        }
    }
}
