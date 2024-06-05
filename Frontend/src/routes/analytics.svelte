<script>
  import { onMount } from "svelte";

  const apiKey = "replace-with-the-api-key"; // replace with the api key!
  const baseApiUrl = "https://app-rssi-api-sea-dev.azurewebsites.net";
  // const baseApiUrl = "https://localhost:7095";
  const earthApiUrl = baseApiUrl + "/api/earthdata/ncei";
  const solarWindApiUrl = baseApiUrl + "/api/satellitedata/ace"
  const predUrl = baseApiUrl + "/api/btregression";

  let earthData = {};
  let solarWindData = {};
  let solarWindPred = {};
  let data = {};

  let reconFreqDay;
  let reconFreqMonth;
  let reconFreqWeek;

  let vector =
  {
    bt_pred : 0,
    bx_pred : 0,
    by_pred : 0,
    bz_pred : 0,
  }

  // Function to fetch Earth data
  async function fetchEarthData() {
    console.log("Fetching geo-magnetic data");
    const response = await fetch(earthApiUrl, {
      headers: {
        "x-api-key": `${apiKey}`,
        "Content-Type": "application/json",
      },
    });

    if (response.ok) {
      earthData = await response.json();
    } else {
      console.error("Failed to fetch geomagnetic data!");
    }
  }

  // Function to fetch solar wind data
  async function fetchSolarWindData() {
    console.log("Fetching solar wind data");
    const response = await fetch(solarWindApiUrl, {
      headers: {
        "x-api-key": `${apiKey}`,
        "Content-Type": "application/json",
      },
    });

    if (response.ok) {
      solarWindData = await response.json();
    } else {
      console.error("Failed to fetch solae wind data!");
    }
  }


  // Function to fetch solar wind data
  async function fetchSolarWindPrediction(wind,earth) {

    var date = new Date();

    const requestBody = {
      year: date.getFullYear(),
      month: date.getMonth() + 1,
      bxGSM: wind.bxGSM,
      byGSM: wind.byGSM,
      bzGSM: wind.bzGSM,
      intensity: earth.intensity,
      declination: earth.declination,
      inclination: earth.inclination,
      east: earth.east,
      north: earth.north,
      vertical: earth.vertical,
      horizontal: earth.horizontal,
      class: 2 /*Class labeled as positive*/
    };
    console.log("Fetching solar wind predictions");

    const response = await fetch(predUrl, {
      method: "POST",
      headers: {
        "x-api-key": `${apiKey}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(requestBody),
    });

    if (response.ok) {
      solarWindPred = await response.json();
    } else {
      console.error("Failed to fetch prediction!");
    }
  }
  
  onMount(async () => {
    // Fetch data from backend
    await fetchSolarWindData();
        await fetchEarthData();

    if(solarWindData != null && earthData != null) {
      await fetchSolarWindPrediction(solarWindData,earthData);
    }
    vector.bt_pred = solarWindPred.bt;

    data = await fetchDscovrData();

    vector.bx_pred = vector.bt_pred * Math.cos(data.theta_gsm) 
      * Math.cos(data.phi_gsm);
    vector.by_pred = vector.bt_pred * Math.cos(data.theta_gsm) 
      * Math.sin(data.phi_gsm);
    vector.bz_pred = vector.bt_pred * Math.sin(data.theta_gsm);

  });

async function fetchDscovrData(){

  let url = "https://services.swpc.noaa.gov/json/dscovr/dscovr_mag_1s.json";

  try {
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (response.ok) {
      // Successfully posted the tweet
      const data = await response.json();
      const lastElement = data[data.length - 1]
      return lastElement;
    } else {
      // Handle any errors during the POST request
      console.error("Failed to fetch data!");
    }
  } catch (error) {
    // Handle network or fetch errors
    console.error("Error :", error);
  }
}

</script>
  
<main>
  <section>
    <h2 style="color:#ffcc00;">
      <i class="fa-solid fa-chart-column" style="color: #ffcc00;"></i>
      AI assisted analytics
    </h2>

    <h3>
      <i class="fa-solid fa-stopwatch fa-beat-fade" style="color: #ffcc00;"></i>
      Reconnection freq
    </h3>

    <h4 class="bullet-point">
      Over The Day: {reconFreqDay}
    </h4>
    <h4 class="bullet-point">
      Over The Week: {reconFreqWeek}
    </h4>
    <h4 class="bullet-point">
      Over The Month: {reconFreqMonth}
    </h4>

    <h3>Most vulnerable conditions</h3>
    {#if !vector.bt_pred}    
      <p>Fetching...</p>
    {/if}
    
    <div class="options-container">
      <button class="round-button">
        Bx<br/>{vector.bx_pred.toFixed(2)}
      </button>
      <button class="round-button">
        By<br/>{vector.by_pred.toFixed(2)}
      </button>
      <button class="round-button">
        Bz<br/>{vector.bz_pred.toFixed(2)}
      </button>
      <button class="round-button">
        Bt<br/>{vector.bt_pred.toFixed(2)}
      </button>
    </div>

    <p>
      Powered by <a href="https://dotnet.microsoft.com/en-us/apps/machinelearning-ai" target="_blank">
        ML.NET
      </a>
    </p>
  </section>
</main>
  
<style>
  .options-container {
    display: flex;
    margin-top: 10px;
  }

  .round-button {
    width: 100px;
    height: 100px;
    border: 2px solid yellow;
    border-radius: 50%;
    margin-right: 10px;
    color: white;
    margin-left: 5px;
    cursor: pointer;
    transition: box-shadow 0.3s ease-in-out, transform 0.3s ease-in-out;
  }

  .round-button:hover {
    box-shadow: 0 0 20px yellow; /* Adjust the glow effect */
    transform: scale(1.1); /* Adjust the scale effect */
  }

  .bullet-point {
    position: relative;
    padding-left: 20px;
  }

  .bullet-point::before {
    content: "";
    display: inline-block;
    width: 10px;
    height: 10px;
    background-color: red;
    border-radius: 50%;
    position: absolute;
    left: 0;
    top: 50%;
    transform: translateY(-50%);
    animation: glow 1s infinite;
  }

  @keyframes glow {
    0% {
      box-shadow: 0 0 5px red;
    }
    50% {
      box-shadow: 0 0 20px red;
    }
    100% {
      box-shadow: 0 0 5px red;
    }
  }

  a {
    color: dodgerblue;
    text-decoration: none;
    font-weight: bold;
  }
</style>

