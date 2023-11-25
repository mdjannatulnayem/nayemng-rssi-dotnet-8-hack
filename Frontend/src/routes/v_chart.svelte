<script>
  import { onMount } from 'svelte';
  import Chart from 'chart.js/auto';
  import Analytics from './analytics.svelte';
  
  let magnetometerChart;
  let orientationChart;

  onMount(() => {
    
    const magnetometerCtx = magnetometerChart.getContext('2d');
    const orientationCtx = orientationChart.getContext('2d');

    
    // Data for the magnetometer chart
    const magnetometerData = {
      labels: ['June', 'July', 'Aug', 'Sept', 'Oct'],
      datasets: [
        {
          label: 'Bt',
          data: [50, 55, 60, 65, 70],
          borderColor: 'rgba(255, 0, 0, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'ByGSM',
          data: [20, 25, 30, 35, 40],
          borderColor: 'rgba(0, 255, 0, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'BxGSM',
          data: [70, 75, 80, 85, 90],
          borderColor: 'rgba(0, 0, 255, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'BzGSM',
          data: [30, 35, 40, 45, 50],
          borderColor: 'rgba(255, 255, 0, 1)',
          borderWidth: 2,
          fill: false,
        },
      ],
    };

    // Data for the orientation chart (intensity, inclination, declination, north, east, vertical, horizontal)
    const orientationData = {
      labels:['June', 'July', 'Aug', 'Sept', 'Oct'],
      datasets: [
        {
          label: 'Intensity',
          data: [100, 110, 120, 130, 140],
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'Inclination',
          data: [10, 15, 20, 25, 30],
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'Declination',
          data: [-10, -15, -20, -25, -30],
          borderColor: 'rgba(255, 0, 0, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'North',
          data: [50, 55, 60, 65, 70],
          borderColor: 'rgba(0, 255, 0, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'East',
          data: [-20, -25, -30, -35, -40],
          borderColor: 'rgba(0, 0, 255, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'Vertical',
          data: [-30, -35, -40, -45, -50],
          borderColor: 'rgba(255, 255, 0, 1)',
          borderWidth: 2,
          fill: false,
        },
        {
          label: 'Horizontal',
          data: [70, 75, 80, 85, 90],
          borderColor: 'rgba(128, 128, 128, 1)',
          borderWidth: 2,
          fill: false,
        },
      ],
    };

    // Adjust the chart height for larger charts
    const chartHeight = 600; // Set the desired height here

    const commonOptions = {
      maintainAspectRatio: true, // Enable aspect ratio
    };

    
    const magnetometerConfig = {
      type: 'line',
      data: magnetometerData,
      options: {
        ...commonOptions,
        scales: {
          y: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Magnetometer Data',
            },
          },
          x: {
            title: {
              display: true,
              text: 'Month',
            },
          },
        },
      },
    };

   
    const orientationConfig = {
      type: 'line',
      data: orientationData,
      options: {
        ...commonOptions,
        scales: {
          y: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Orientation Data',
            },
          },
          x: {
            title: {
              display: true,
              text: 'Month',
            },
          },
        },
      },
    };

    const resizeCharts = () => {
      magnetometerChart.height = chartHeight;
      orientationChart.height = chartHeight;
    };

    new Chart(magnetometerCtx, magnetometerConfig);
    new Chart(orientationCtx, orientationConfig);
    

    // Resize charts initially
    resizeCharts();
  });
</script>

<style>
  .chart-container {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 20px;
  }

  .chart-items {
    width: 100%;
  }

  .analytics {
    display: block;
    padding: 10px 40px;
  }
</style>

<div class="analytics"><Analytics/></div>

<br/>

<div class="chart-container">
  
  <div class="chart-items">
    <canvas bind:this={magnetometerChart} id="magnetometerChart"></canvas>
  </div>
  <div class="chart-items">
    <canvas bind:this={orientationChart} id="orientationChart"></canvas>
  </div>
</div>
