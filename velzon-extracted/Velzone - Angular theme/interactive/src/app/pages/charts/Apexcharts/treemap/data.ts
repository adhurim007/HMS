import { ChartOptions } from "./treemap.model";

/**
 * Basic Treemap Chart
 */
const basicTreemapChart: ChartOptions = {
  series: [{
    data: [{
        x: "New Delhi",
        y: 218,
      },
      {
        x: "Kolkata",
        y: 149,
      },
      {
        x: "Mumbai",
        y: 184,
      },
      {
        x: "Ahmedabad",
        y: 55,
      },
      {
        x: "Bangaluru",
        y: 84,
      },
      {
        x: "Pune",
        y: 31,
      },
      {
        x: "Chennai",
        y: 70,
      },
      {
        x: "Jaipur",
        y: 30,
      },
      {
        x: "Surat",
        y: 44,
      },
      {
        x: "Hyderabad",
        y: 68,
      },
      {
        x: "Lucknow",
        y: 28,
      },
      {
        x: "Indore",
        y: 19,
      },
      {
        x: "Kanpur",
        y: 29,
      },
    ],
  }, ],
  legend: {
    show: false,
  },
  chart: {
    height: 350,
    type: "treemap",
    toolbar: {
      show: false,
    },
  },
  colors: ["#867df2"],
  title: {
    text: "Basic Treemap",
    style: {
      fontWeight: 500,
    },
  },
};

/**
 * Multi-Dimensional Treemap Chart
 */
const MultiDimensionalChart: ChartOptions = {
  series: [{
    name: "Desktops",
    data: [{
        x: "ABC",
        y: 10,
      },
      {
        x: "DEF",
        y: 60,
      },
      {
        x: "XYZ",
        y: 41,
      },
    ],
  },
  {
    name: "Mobile",
    data: [{
        x: "ABCD",
        y: 10,
      },
      {
        x: "DEFG",
        y: 20,
      },
      {
        x: "WXYZ",
        y: 51,
      },
      {
        x: "PQR",
        y: 30,
      },
      {
        x: "MNO",
        y: 20,
      },
      {
        x: "CDE",
        y: 30,
      },
    ],
  },
  ],
  legend: {
    show: false,
  },
  chart: {
    height: 350,
    type: "treemap",
    toolbar: {
      show: false,
    },
  },
  title: {
    text: "Multi-dimensional Treemap",
    align: "center",
    style: {
      fontWeight: 500,
    },
  },
  colors: ["#8a82f2", "#94e5ee"],
};

/**
 * Distributed Treemap Chart
 */
const distributedTreemapChart: ChartOptions = {
  series: [{
    data: [{
        x: "New Delhi",
        y: 218,
      },
      {
        x: "Kolkata",
        y: 149,
      },
      {
        x: "Mumbai",
        y: 184,
      },
      {
        x: "Ahmedabad",
        y: 55,
      },
      {
        x: "Bangaluru",
        y: 84,
      },
      {
        x: "Pune",
        y: 31,
      },
      {
        x: "Chennai",
        y: 70,
      },
      {
        x: "Jaipur",
        y: 30,
      },
      {
        x: "Surat",
        y: 44,
      },
      {
        x: "Hyderabad",
        y: 68,
      },
      {
        x: "Lucknow",
        y: 28,
      },
      {
        x: "Indore",
        y: 19,
      },
      {
        x: "Kanpur",
        y: 29,
      },
    ],
  }, ],
  legend: {
    show: false,
  },
  chart: {
    height: 350,
    type: "treemap",
    toolbar: {
      show: false,
    },
  },
  title: {
    text: "Distibuted Treemap (different color for each cell)",
    align: "center",
    style: {
      fontWeight: 500,
    },
  },
  colors: [
    "#7f76f1", "#6ea6f8", "#34d8c2", "#88e1ec", "#ffc979",
    "#ff925d", "#7f76f1", "#6ea6f8", "#34d8c2", "#88e1ec", "#ffc979", "#ff925d", "#7f76f1"
  ],
  plotOptions: {
    treemap: {
      distributed: true,
      enableShades: false,
    },
  },
};

/**
 * Treemap Chart with Color Ranges
 */
const colorRangesChart: ChartOptions = {
  series: [{
    data: [{
        x: "INTC",
        y: 1.2,
      },
      {
        x: "GS",
        y: 0.4,
      },
      {
        x: "CVX",
        y: -1.4,
      },
      {
        x: "GE",
        y: 2.7,
      },
      {
        x: "CAT",
        y: -0.3,
      },
      {
        x: "RTX",
        y: 5.1,
      },
      {
        x: "CSCO",
        y: -2.3,
      },
      {
        x: "JNJ",
        y: 2.1,
      },
      {
        x: "PG",
        y: 0.3,
      },
      {
        x: "TRV",
        y: 0.12,
      },
      {
        x: "MMM",
        y: -2.31,
      },
      {
        x: "NKE",
        y: 3.98,
      },
      {
        x: "IYT",
        y: 1.67,
      },
    ],
  }, ],
  legend: {
    show: false,
  },
  chart: {
    height: 350,
    type: "treemap",
    toolbar: {
      show: false,
    },
  },
  title: {
    text: "Treemap with Color scale",
    style: {
      fontWeight: 500,
    },
  },
  dataLabels: {
    enabled: true,
    style: {
      fontSize: "12px",
    },
    offsetY: -4,
  },
  plotOptions: {
    treemap: {
      enableShades: true,
      shadeIntensity: 0.5,
      reverseNegativeShade: true,
      colorScale: {
        ranges: [{
            from: -6,
            to: 0,
            color: "#70b4bd",
          },
          {
            from: 0.001,
            to: 6,
            color: "#8b83f3",
          },
        ],
      },
    },
  },
};

export { basicTreemapChart, MultiDimensionalChart, distributedTreemapChart, colorRangesChart }
