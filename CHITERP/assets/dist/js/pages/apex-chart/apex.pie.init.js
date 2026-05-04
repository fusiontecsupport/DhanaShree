$(function () {

    var url = '/Home/GetChart1';

    $.getJSON(url, function (response) {
        console.log(response);
        var basicdoughnutChart = echarts.init(document.getElementById('basic-doughnut'));
        var option = {
            // Add title
            title: {
                text: '',
                subtext: '',
                x: 'center'
            },

            // Add legend
            legend: {
                orient: 'vertical',
                x: 'left',
                data: response
            },

            // Add custom colors
            color: ['#212529', '#ffbc34', '#4fc3f7', '#f62d51', '#2962FF'],

            // Display toolbox
            toolbox: {
                show: true,
                orient: 'vertical',
                feature: {
                    mark: {
                        show: true,
                        title: {
                            mark: 'Markline switch',
                            markUndo: 'Undo markline',
                            markClear: 'Clear markline'
                        }
                    },
                    dataView: {
                        show: true,
                        readOnly: true,
                        title: 'View data',
                        lang: ['View chart data', 'Close', 'Update']
                    },
                    magicType: {
                        show: true,
                        title: {
                            pie: 'Switch to pies',
                            funnel: 'Switch to funnel',
                        },
                        type: ['pie', 'funnel'],
                        option: {
                            funnel: {
                                x: '25%',
                                y: '20%',
                                width: '50%',
                                height: '70%',
                                funnelAlign: 'left',
                                max: 1548
                            }
                        }
                    },
                    restore: {
                        show: true,
                        title: 'Restore'
                    },
                    saveAsImage: {
                        show: true,
                        title: 'Same as image',
                        lang: ['Save']
                    }
                }
            },

            // Enable drag recalculate
            calculable: true,

            // Add series
            series: [
                {
                    name: 'Sales',
                    type: 'pie',
                    radius: ['50%', '70%'],
                    center: ['50%', '57.5%'],
                    itemStyle: {
                        normal: {
                            label: {
                                show: true
                            },
                            labelLine: {
                                show: true
                            }
                        },
                        emphasis: {
                            label: {
                                show: true,
                                formatter: '{b}' + '\n\n' + '{c} ({d}%)',
                                position: 'center',
                                textStyle: {
                                    fontSize: '9.5',
                                    fontWeight: '500'
                                }
                            }
                        }
                    },

                    data: response
                }
            ]
        };

        basicdoughnutChart.setOption(option);
       
        
    });

    var url2 = '/Home/GetChart2';
    $.getJSON(url2, function (response) {
        console.log(response);
        //Morris.Bar({
        //    element: 'morris-bar-chart1',
        //    data: response,
        //    xkey: 'y',
        //    ykeys: ['a'],
        //    labels: ['Invoice Count'],
        //    barColors: ["#ea5545", "#f46a9b", "#ef9b20", "#edbf33", "#ede15b", "#bdcf32", "#87bc45", "#27aeef", "#b33dc6"],
        //    hideHover: 'auto',
        //    gridLineColor: '#eef0f2',
        //    resize: true
        //});
        //Morris.Bar({
        //    element: 'morris-bar-chart2',
        //    data: response,
        //    xkey: 'y',
        //    ykeys: ['b'],
        //    labels: ['Product Count'],
        //    barColors: ["#edbf33", "#ede15b", "#bdcf32", "#87bc45", "#27aeef", "#b33dc6", "#ea5545", "#f46a9b", "#ef9b20"],
        //    hideHover: 'auto',
        //    gridLineColor: '#eef0f2',
        //    resize: true
        //});
        Morris.Bar({
            element: 'morris-bar-chart3',
            data: response,
            xkey: 'y',
            ykeys: ['c'],
            labels: ['Product Value (in Lakhs)'],
            barColors: ["#87bc45", "#27aeef", "#b33dc6", "#ea5545", "#f46a9b", "#ef9b20", "#edbf33", "#ede15b", "#bdcf32"],
            hideHover: 'auto',
            gridLineColor: '#eef0f2',
            resize: true
        });

    });

    var url3 = '/Home/GetChart3';
    $.getJSON(url3, function (response) {
        Morris.Area({
            element: 'extra-area-chart',
            data: response,
            lineColors: ['#fc4b6c', '#00acc1', '#1e88e5', "#ef9b20", "#edbf33", "#ede15b", "#bdcf32"],
            xkey: 'period',
            ykeys: ['y'],
            labels: ['AMC', 'Billed Machines', 'Consumables and Spares', 'Machines', 'Perday', 'Rental invoice', 'Rental Machine Qty'],
            pointSize: 0,
            lineWidth: 0,
            resize: true,
            fillOpacity: 0.8,
            behaveLikeLine: true,
            gridLineColor: '#e0e0e0',
            hideHover: 'auto'

        });
    });
});
//hljs.initHighlightingOnLoad();