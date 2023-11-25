<!-- src/routes/index.svelte -->

<script>
    import { Link } from "svelte-routing";
    import Home from "./home.svelte";
    import VChart from "./v_chart.svelte";

    let satelliteData = [
        { name: "ACE", position: "L1", status: "Active" },
        { name: "DSCOVR", position: "L1", status: "Active" },
    ];

    // Create a variable to track which component to display
    let displayHome = false;
    let displayIndex = true; // Display the index component by default

    // Function to display the Home component
    function showHome() {
        displayHome = true;
        displayIndex = false;
    }

    // Function to display the index.svelte component
    function showIndex() {
        displayHome = false;
        displayIndex = true;
    }
</script>

<main class="container">
    <div class="navbar">
        <h1>Recon Solar Shield Initiative</h1>
        <div class="navbar-buttons">
            <!-- Link to show the Home component -->
            <button
                id="home-button"
                on:click={showHome}
                style="background-color: #444; font-size: 20px; color: #ffcc00; cursor: pointer;"
            >
                Main
            </button>
            <!-- Button to show the Chart component -->
            <button
                on:click={showIndex}
                style="background-color: #444; font-size: 20px; color: #ffcc00; cursor: pointer;"
            >
                Visualization
            </button>
            
            <!-- Route to Twitter -->
            <a href="https://twitter.com/RssiSpaceApps23" target="_blank">
                <i class="fa-brands fa-x-twitter" style="color: #ffcc00;background-color:#444;"></i>
            </a>
        </div>
    </div>

    <section class="sat-info">
        <h2>Satellite Information</h2>

        <ul>
            {#each satelliteData as satellite}
                <li>
                    <strong>{satellite.name}:</strong>
                    {satellite.position}, {satellite.status}
                </li>
            {/each}
        </ul>

        {#if displayHome}
            <!-- Render the Home component when displayHome is true -->
            <Home />
        {/if}

        {#if displayIndex}
            <!-- Render the index.svelte component when displayIndex is true -->
            <!-- Add code for the index component here -->
            <VChart/>
        {/if}

        <!-- Add code for the satellite information here -->
    </section>
</main>

<style>
    /* Add your CSS styles here */
    .container {
        min-height: 100vh;
        display: flex;
        flex-direction: column;
        background-color: #333;
        color: #fff;
        margin: 30px;
    }

    .navbar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        font-size: 24px;
        margin-bottom: 20px;
        width: 100%;
        background-color: #444; /* Slightly lighter background */
        padding: 10px;
    }

    .navbar h1 {
        color: #ffcc00;
        font-size: 36px;
        margin: 0;
        background-color: #444;
    }

    .navbar-buttons {
        display: flex;
        gap: 20px;
        background-color: #444;
        margin-right: 10px;
    }

    .navbar-buttons button ,a {
        border: none;
        font-size: 24px;
        color: #ffcc00;
        cursor: pointer;
        background-color: #444;
        padding: 5px;
    }

    a {
        text-decoration: none;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        margin-bottom: 10px;
    }

    strong {
        color: #ffcc00;
    }
</style>