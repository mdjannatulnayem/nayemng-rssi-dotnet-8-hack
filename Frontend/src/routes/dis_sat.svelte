<!-- src/routes/dis_sat.svelte -->

<script>
    import { onMount } from "svelte";

    const apiKey = "replace-with-the-api-key"; // replace with the api key!
    const baseApiUrl = "https://app-rssi-api-sea-dev.azurewebsites.net";
    // const baseApiUrl = "https://localhost:7095";
    const earthApiUrl = baseApiUrl + "/api/earthdata/ncei";
    const solarWindApiUrl = baseApiUrl + "/api/satellitedata/dscovr"

    let earthData = {};
    let solarWindData = {};

    // Function to fetch Earth data from the API
    async function fetchEarthData() {
        console.log("Fetching geo-magnetic data.");
        const response = await fetch(earthApiUrl, {
            headers: {
                "x-api-key": `${apiKey}`,
                "Content-Type": "application/json",
            },
        });

        if (response.ok) {
            earthData = await response.json();
        } else {
            console.error("Failed to fetch Earth data from the API.");
        }
    }

    // Function to fetch Sun data from the API
    async function fetchSolarWindData() {
        console.log("Fetching solar wind data from DSCOVR.");
        const response = await fetch(solarWindApiUrl, {
            headers: {
                "x-api-key": apiKey,
                "Content-Type": "application/json",
            },
        });

        if (response.ok) {
            solarWindData = await response.json();
        } else {
            console.error("Failed to fetch Sun data from the API.");
        }
    }

    onMount(async () => {
        // Fetch Earth and Sun data 
        // when the component is mounted
        await fetchEarthData();
        await fetchSolarWindData();
        // fetch dscovr data every 10s
        setInterval(fetchSolarWindData,10000);
    });
</script>

<main class="dis-sat-container">
    <!-- Left Floating Items -->

    <div style=" font-size:18px" class="left-float">
        <h4>Solar wind
            <br/>
            <span style="color: #ffcc00">(DSCOVR)</span>
        </h4>

        <div class="status-icon">
            <span class="square" />
            <span class="status-label">bx_gsm:</span>
            <span class="status-text">
                <span class="bx-gsm-value">{solarWindData.bxGSM}</span>
            </span>
        </div>

        <div class="status-icon">
            <span class="square" />
            <span class="status-label">by_gsm:</span>
            <span class="status-text">
                <span class="by-gsm-value">{solarWindData.byGSM}</span>
            </span>
        </div>

        <div class="status-icon">
            <span class="square" />
            <span class="status-label">bz_gsm:</span>
            <span class="status-text">
                <span class="bz-gsm-value">{solarWindData.bzGSM}</span>
            </span>
        </div>

        <div class="status-icon">
            <span class="square" />
            <span class="status-label">bt:</span>
            <span class="status-text">
                <span class="bt-value">{solarWindData.bt}</span>
            </span>
        </div>

        <h4>Geo-magnetic field</h4>

        <div class="left-float">
            
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Latitude:</span>
                <span class="status-text">{earthData.latitude}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Longitude:</span>
                <span class="status-text">{earthData.longitude}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Altitude:</span>
                <span class="status-text">{earthData.altitude}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Intensity:</span>
                <span class="status-text">{earthData.intensity}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Declination:</span>
                <span class="status-text">{earthData.declination}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Inclination:</span>
                <span class="status-text">{earthData.inclination}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">North:</span>
                <span class="status-text">{earthData.north}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">East:</span>
                <span class="status-text">{earthData.east}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Vertical:</span>
                <span class="status-text">{earthData.vertical}</span>
            </div>
            <div class="status-icon">
                <span class="square" />
                <span class="status-label">Horizontal:</span>
                <span class="status-text">{earthData.horizontal}</span>
            </div>
        </div>
    </div>

    <!-- Gap between left and right float items -->
    <div class="gap" />

    <!-- Right Floating Items -->
    <div class="right-float">
        <!-- Info Container -->
        <div class="info-container">
            <!-- Info Grids (3x3 Format) -->
            <div class="info-grid">
                <!-- Info Item 1 -->

                <div
                    style="float: right; margin-left: 20px; display: grid; grid-template-columns: repeat(3, 1fr); grid-gap: 20px;"
                    class="info-container"
                >
                    <div class="info-item">
                        <h2>Solar Wind Monitoring</h2>
                        <ul class="data-fields">
                            <li>
                                <span class="bullet" /> Solar Wind Speed: 500 km/s
                            </li>
                            <li>
                                <span class="bullet" /> Solar Wind Density: 5 particles/cm³
                            </li>
                            <li>
                                <span class="bullet" /> Solar Wind Magnetic Field:
                                10 nT
                            </li>
                        </ul>
                    </div>

                    <div class="info-item">
                        <h2>Earth Radiation Budget</h2>
                        <ul class="data-fields">
                            <li>
                                <span class="bullet" /> Earth's Albedo: 0.30
                            </li>
                            <li><span class="bullet" /> Cloud Cover: 40%</li>
                        </ul>
                    </div>

                    <div class="info-item">
                        <h2>Space Weather Alerts</h2>
                        <ul class="data-fields">
                            <li>
                                <span class="bullet" /> Solar Flares: Moderate
                            </li>
                            <li>
                                <span class="bullet" /> Geomagnetic Storms: None
                            </li>
                        </ul>
                    </div>

                    <div class="info-item">
                        <h2>Climate Research</h2>
                        <ul class="data-fields">
                            <li>
                                <span class="bullet" /> Albedo Changes: 0.02 per
                                year
                            </li>
                            <li>
                                <span class="bullet" /> Cloud Cover Patterns: Variable
                            </li>
                        </ul>
                    </div>

                    <div class="info-item">
                        <h2>Atmospheric Measurements</h2>
                        <ul class="data-fields">
                            <li>
                                <span class="bullet" /> Ozone Levels: 300 Dobson
                                Units
                            </li>
                            <li>
                                <span class="bullet" /> Aerosol Concentrations: 15
                                µg/m³
                            </li>
                        </ul>
                    </div>

                    <div class="info-item">
                        <h2>Space Environment Research</h2>
                        <ul class="data-fields">
                            <li><span class="bullet" /> Auroras: Active</li>
                            <li>
                                <span class="bullet" /> Geomagnetic Disturbances:
                                Low
                            </li>
                        </ul>
                    </div>

                </div>
                <!-- Add the remaining Info Items here -->
            </div>  
        </div>
    </div>    
       
</main>

<style>
    /* Add your CSS styles here */
    .dis-sat-container {
        display: flex;
        justify-content: space-between;
    }


    .left-float {
        float: left;
        min-width: 20%;
        /* Add styles for left floating items */
    }

    .right-float {
        float: right;
        margin-top: 80px;
    }

    .gap {
        width: 20px; /* Adjust the width of the gap as needed */
        float: left;
    }

    h2 {
        color: #ffcc00;
    }
    /* Add styles for info-container, info-grid, info-item, data-fields, etc. */
</style>
