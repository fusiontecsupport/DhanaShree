$(function () {

    var base_url = window.location.origin;// + '/dashboard/';
    var url = base_url + '/Dashboard/GetChart1';

    $.getJSON(url, function (response) {
        //console.log(response);
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

    var url2 = base_url + '/Dashboard/GetChart2';
    $.getJSON(url2, function (response) {
        //console.log(response);
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
    // ==============================================================
    // Monthwise Product Type Wiset Revenue
    // ==============================================================

    var url3 = base_url + '/Dashboard/GetChart3';

    $.getJSON(url3, function (response) {
        //console.log(response);
        mcval = [];
        amcval = [];
        consval = [];
        totval = [];
        var t = response.length;
        //console.log(t);
        var mctot = 0;
        var amctot = 0;
        var constot = 0;
        var tot = 0;
        for (var i = 0; i < t; i++) {
            //console.log(i);
            //console.log(response[i]["Machines_Value"]);

            mcval[i] = response[i]["Machines_Value"];
            amcval[i] = response[i]["AMC"];
            consval[i] = response[i]["Consumables_and_Spares"];
            totval[i] = response[i]["Total_Value"];

            mctot = mctot + mcval[i];
            amctot = amctot + amcval[i];
            constot = constot + consval[i];
            tot = tot + totval[i];

        }
        $(".mcval").text(mctot.toFixed(0) + " Lakhs.");
        $(".amcval").text(amctot.toFixed(0) + " Lakhs.");
        $(".consval").text(constot.toFixed(0) + " Lakhs.");
        $(".totval").text(tot.toFixed(0) + " Lakhs.");
        //console.log(mcval);
        $('#mcvaluechart').sparkline(mcval, {
            type: 'bar',
            height: '35',
            barWidth: '4',
            resize: true,
            barSpacing: '4',
            barColor: '#1e88e5'
        });
        $('#amcvaluechart').sparkline(amcval, {
            type: 'bar',
            height: '35',
            barWidth: '4',
            resize: true,
            barSpacing: '4',
            barColor: '#7460ee'
        });
        $('#consvaluechart').sparkline(consval, {
            type: 'bar',
            height: '35',
            barWidth: '4',
            resize: true,
            barSpacing: '4',
            barColor: '#7460ee'
        });
        $('#totvaluechart').sparkline(totval, {
            type: 'bar',
            height: '35',
            barWidth: '4',
            resize: true,
            barSpacing: '4',
            barColor: '#7460ee'
        });
        var sparkResize;
    });


    var url4 = base_url + '/Dashboard/GetChart4';

    $.getJSON(url4, function (response) {
        //console.log(response);
        pieval = [];
        var t = response.length;
        for (var i = 0; i < t; i++) {
            pieval[i] = response[i]["Total_Value"];
        }
        $('#sparklinepie').sparkline(pieval, {
            type: 'pie',
            height: '200',
            resize: true,
            sliceColors: ['#1e88e5', '#fc4b6c', '#f1f2f7', '#7460ee']
        });
        var sparkResize;
    });


    //var url6 =  base_url + '/Dashboard/GetChart6';
    //$.getJSON(url3, function (response) {
    //    Morris.Area({
    //        element: 'extra-area-chart',
    //        data: response,
    //        lineColors: ['#fc4b6c', '#00acc1', '#1e88e5', "#ef9b20", "#edbf33", "#ede15b", "#bdcf32"],
    //        xkey: 'period',
    //        ykeys: ['y'],
    //        labels: ['AMC', 'Billed Machines', 'Consumables and Spares', 'Machines', 'Perday', 'Rental invoice', 'Rental Machine Qty'],
    //        pointSize: 0,
    //        lineWidth: 0,
    //        resize: true,
    //        fillOpacity: 0.8,
    //        behaveLikeLine: true,
    //        gridLineColor: '#e0e0e0',
    //        hideHover: 'auto'

    //    });
    //});



});
//hljs.initHighlightingOnLoad();