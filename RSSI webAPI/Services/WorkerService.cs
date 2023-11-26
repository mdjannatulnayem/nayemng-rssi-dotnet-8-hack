using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RSSI_webAPI.Models;
using RSSI_webAPI.Models.DtoModels;
using System.Text;
using Tweetinvi.Models;
using Tweetinvi;


namespace RSSI_webAPI.Services;

public class WorkerService : BackgroundService
{
    readonly HttpClient _client;
    readonly ILogger<WorkerService> _log;

    SatelliteDataModel? satData = new();
    GeoMagnetDataModel? earthData = new();

    int delay;

    private readonly IConfiguration _conf;
    private readonly string _apiKey;
    private readonly string _apiKeySecret;
    private readonly string _accessToken;
    private readonly string _accessTokenSecret;

    public WorkerService(
        ILogger<WorkerService> log,
        IHttpClientFactory cf,
        IConfiguration conf
    ){
        _log = log;
        _client = cf.CreateClient();
        _conf = conf;
        _apiKey = _conf["Xbot:ApiKey"];
        _apiKeySecret = _conf["Xbot:ApiKeySecret"];
        _accessToken = _conf["Xbot:AccessToken"];
        _accessTokenSecret = _conf["Xbot:AccessTokenSecret"];
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        DateTime t = DateTime.UtcNow;
        while (!token.IsCancellationRequested)
        {
            try {
                satData = await GetSatelliteData();
                earthData = await GetGeoMagneticData();

                if (satData == null || earthData == null)
                {
                    delay = 1000;
                    continue;
                }else{
                    delay = 60000;
                }

                if (earthData.Vertical == satData.BzGSM && satData.BzGSM < 0)
                {
                    _log.LogInformation("{t} : Reconnection alert !!!", t);

                    await PostTweet(new TweetReqDtoModel
                    {
                        Text = $"{t}\nMagnetic reconnection is imminent!",
                    });

                    await SaveReconEventInfo(new ReconDataModel
                    {
                        Year = t.Year,
                        Month = t.Month,
                        BxGSM = Convert.ToSingle(
                            satData.BxGSM
                        ),
                        ByGSM = Convert.ToSingle(
                            satData.ByGSM
                        ),
                        BzGSM = Convert.ToSingle(
                            satData.BzGSM
                        ),
                        Bt = Convert.ToSingle(
                            satData.Bt
                        ),
                        Intensity = Convert.ToSingle(
                            earthData.Intensity
                        ),
                        Declination = Convert.ToSingle(
                            earthData.Declination
                        ),
                        Inclination = Convert.ToSingle(
                            earthData.Inclination
                        ),
                        North = Convert.ToSingle(
                            earthData.North
                        ),
                        East = Convert.ToSingle(
                            earthData.East
                        ),
                        Vertical = Convert.ToSingle(
                            earthData.Vertical
                        ),
                        Horizontal = Convert.ToSingle(
                            earthData.Horizontal
                        )
                    });

                }

                // Done!
            } catch (Exception ex) {
                _log.LogError(ex.Message);
            } finally{
                await Task.Delay(3 * delay, token);
            }
        }
    }

    private async Task<GeoMagnetDataModel?> GetGeoMagneticData()
    {
        try
        {
            GeoMagnetDataModel? data = null;

            double northMagPoleLat = 86.50;
            double northMagPoleLon = 164.04;
            var date = DateTime.UtcNow;

            string url = $"https://www.ngdc.noaa.gov/geomag-web/calculators/calculateIgrfwmm?lat1={northMagPoleLat}&lon1={northMagPoleLon}&model=WMM&startYear={date.Year}&endYear={date.Year}&key=EAU2y&resultFormat=json";

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(responseBody);

                JToken? result = jsonObject["result"]?.FirstOrDefault();

                if (result != null)
                {
                    data = new GeoMagnetDataModel
                    {
                        Time = date,
                        Latitude = result["latitude"].ToObject<double>(),
                        Longitude = result["longitude"].ToObject<double>(),
                        Altitude = result["elevation"].ToObject<double>(),
                        Intensity = result["totalintensity"].ToObject<double>(),
                        Declination = result["declination"].ToObject<double>(),
                        Inclination = result["inclination"].ToObject<double>(),
                        North = result["xcomponent"].ToObject<double>(),
                        East = result["ycomponent"].ToObject<double>(),
                        Vertical = result["zcomponent"].ToObject<double>(),
                        Horizontal = result["horintensity"].ToObject<double>()
                    };
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            _log.LogError(ex.Message);
            return null;
        }
    }

    private async Task<SatelliteDataModel?> GetSatelliteData()
    {
        try
        {
            SatelliteDataModel? model = null;

            string url = @"https://services.swpc.noaa.gov/products/solar-wind/mag-5-minute.json";

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var solarWindData = JsonConvert.DeserializeObject<object[][]>(responseBody);

                var len = solarWindData.Length;

                if (len == 1)
                    return model;

                model = new SatelliteDataModel
                {
                    Time = DateTime.UtcNow,
                    // Time = DateTime.Parse(solarWindData[len - 1][0].ToString()),
                    Bt = Convert.ToDouble(solarWindData[len - 1][6]),
                    BxGSM = Convert.ToDouble(solarWindData[len - 1][1]),
                    ByGSM = Convert.ToDouble(solarWindData[len - 1][2]),
                    BzGSM = Convert.ToDouble(solarWindData[len - 1][3]),
                };

            }

            return model;
        }
        catch (Exception ex)
        {
            _log.LogError(ex.Message);
            return null;
        }
    }


    private async Task PostTweet(TweetReqDtoModel newTweet)
    {
        var client = new TwitterClient(_apiKey, _apiKeySecret, _accessToken, _accessTokenSecret);
        await client.Execute.AdvanceRequestAsync(
            BuildTwitterRequest(newTweet, client)
        );

    }


    private static Action<ITwitterRequest> BuildTwitterRequest(
    TweetReqDtoModel tweet, TwitterClient client)
    {
        return (ITwitterRequest request) =>
        {
            var json = client.Json.Serialize(tweet);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Query.Url = "https://api.twitter.com/2/tweets";
            request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
            request.Query.HttpContent = content;
        };
    }

    private async Task SaveReconEventInfo(ReconDataModel data)
    {
        string url = @"https://app-rssi-api-eastus-dev-001.azurewebsites.net/api/btregression/feedback";

        var json = JsonConvert.SerializeObject(data);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url,payload);

        if (response.IsSuccessStatusCode)
        {
            _log.LogInformation("Reconnect event saved to database successfully.");
        }
    }

    ///
}