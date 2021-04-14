using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class TestManager
    {
        private TestContext Database { get; }
        public TestManager(TestContext dbContext)
        {
            Database = dbContext;
        }

        public async Task<TestView> SaveNewItemAsync(TestForm newItem)
        {
            Test itemToSave = new Test(newItem);
            int saveResult = await Database.InsertNewAsync(itemToSave);

            return saveResult == 1 ? itemToSave.AsView() : null;
        }

        public async Task<List<TestView>> GetItemAsync()
        {
            List<Test> items = await Database.SelectAllAsync();
            return items?.ConvertAll(c => c.AsView());
        }

        public async Task<bool> DeleteItemByIdentifierAsync(string identifier)
        {
            int result = await Database.DeleteByNameAsync(identifier);
            return result == 1;
        }
    }
}
