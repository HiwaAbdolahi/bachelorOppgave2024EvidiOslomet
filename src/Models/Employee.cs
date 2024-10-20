using Newtonsoft.Json;

namespace bachelorOppgave13.Models;

public class Employee
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("personId")]
    public Guid personId { get; set; }

    [JsonProperty("personFirstName")]
    public string personFirstName { get; set; }

    [JsonProperty("personLastName")]
    public string personLastName { get; set; }

    [JsonProperty("imageUrl")]
    public string[] imageUrl { get; set; }

    [JsonProperty("checkedIn")]
    public Boolean checkedIn { get; set; }

    [JsonProperty("checkInTime")]
    public string checkInTime { get; set; }
    
    [JsonProperty("department")]
    public string department { get; set; }
    
    [JsonProperty("role")]
    public string role { get; set; }
    
}