using Databases_Comparison.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Databases_Comparison.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataModelController : ControllerBase
    {
        
        // Cosmos DB details, In real use cases, these details should be configured in secure configuraion file.
        private readonly string CosmosDBAccountUri = "";
        private readonly string CosmosDBAccountPrimaryKey = "";
        private readonly string CosmosDbName = "UsersDB";
        private readonly string CosmosDbContainerName = "Users";

        private Container ContainerClient()
        {

            CosmosClient cosmosDbClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey);
            Container containerClient = cosmosDbClient.GetContainer(CosmosDbName, CosmosDbContainerName);
            return containerClient;

        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetails()
        {
            try
            {
                var container = ContainerClient();
                var sqlQuery = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<DataModel> queryResultSetIterator = container.GetItemQueryIterator<DataModel>(queryDefinition);


                List<DataModel> employees = new List<DataModel>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<DataModel> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (DataModel employee in currentResultSet)
                    {
                        employees.Add(employee);
                    }
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(DataModel employee)
        {
            try
            {
                var container = ContainerClient();
                var response = await container.CreateItemAsync(employee, new PartitionKey(employee.Id));

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(string empId, string partitionKey)
        {

            try
            {

                var container = ContainerClient();
                var response = await container.DeleteItemAsync<DataModel>(empId, new PartitionKey(partitionKey));
                return Ok(response.StatusCode);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
