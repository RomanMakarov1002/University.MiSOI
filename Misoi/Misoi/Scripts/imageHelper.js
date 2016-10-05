$(function() {
    $('#imgLoad').on('change', function () {
        var img = this.files[0];
        var $loadedimg = $('#loadedImg');
        var reader = new FileReader();
        reader.onloadend = function () {
            $loadedimg.attr('src', reader.result);
            var img = new Image();
            img.src = reader.result;
            $('#width').val(img.width);
            $('#height').val(img.height);
        }
        if (img) {
            reader.readAsDataURL(img);
        } else {
            $loadedimg.src = "";
        }
    });

    $('#btnImgLoad').on('click', function() {
        $('#imgLoad').click();
    });

    $('#showChart').on('click', function() {
        $(this).hide();
        var id = $('#modelId').val();
        $.ajax({
            type: "POST",
            url: "/Home/GetChart/"+id,
            dataType: "json",
            success: drawChart
        });
    });

    function drawChart(res) {
        drawSvgChart(res, "imageChart", "imageContainer");
    }

    function drawNegativeChart(res) {
        drawSvgChart(res, "negativeChart", "negativeChartContainer");
    }

    function drawFilteredChart(res) {
        drawSvgChart(res, "filteredChart", "filteredChartContainer");
    }

    function drawSvgChart(res, name, contname) {
        var $chart = $('#' + name);
        for (var i = 0; i < res.Brightness.length; i++) {
            $chart.append('<g class="bar"> <rect width="3" height="' + (res.BrightnessPersentage[i] + 1)*4 + '" x="' + +((i * 4) + 39) + '" y="' + (100 - res.BrightnessPersentage[i])*4 + '"> </rect> </g>');
        }
        $chart.append('<g class="bar-line">' +
            '<line x1="35" x2="35" y1="0" y2="410"></line> </g>');
        $chart.append('<g class="bar-line">' +
            '<line x1="35" x2="1063" y1="410" y2="410"></line> </g>');
        $chart.append('<g class="y-bar">'+
            '<text x="0" y="10">' + res.MaxPersentage + '%</text>' +
            '<text x="0" y="110">' + Math.round(res.MaxPersentage / 4 * 3 * 100) / 100 + '%</text>' +
            '<text x="0" y="210">' + Math.round(res.MaxPersentage / 2 * 100) / 100 + '%</text>' +
            '<text x="0" y="310">' + Math.round(res.MaxPersentage / 4 * 100) / 100 + '%</text>' +
            '<text x="0" y="410">0</text>' +
            '</g>');
        $chart.append('<g class="x-bar">' +
            '<text x="39" y="420">0</text>' +
            '<text x="295" y="420">64</text>' +
            '<text x="551" y="420">128</text>' +
            '<text x="807" y="420">192</text>' +
            '<text x="1063" y="420">255</text>' +
            '</g>');
        $('#'+contname).removeClass('hide-container');
        $("#" + contname).html($("#" + contname).html());
    }

    $('#addNegative').on('click', function () {
        $(this).hide();
        var id = $('#modelId').val();
        $('#negativeContainer').addClass('loading-img');
        $.ajax({
            type: "POST",
            url: "/Home/GetNegativeImg/" + id,
            dataType: "json",
            success: drawNegativeImg
        });
    });

    function drawNegativeImg(img) {
        $('#negativeContainer').removeClass('loading-img');
        $('#negativeImg').attr('src', img);
        $('#showChartForNegative').css('visibility', 'visible');
    }

    $('#addFilter').on('click', function() {
        $(this).hide();
        var id = $('#modelId').val();
        $('#filteredContainer').addClass('loading-img');
        $.ajax({
            type: "POST",
            url: "/Home/GetFilteredImg/" + id,
            dataType: "json",
            success: drawFilteredImg
        });
    });

    function drawFilteredImg(img) {
        $('#filteredContainer').removeClass('loading-img');
        $('#filteredImg').attr('src', img);
        $('#showChartForFilter').css('visibility', 'visible');
    }

    $('#showChartForNegative').on('click', function () {
        $(this).hide();
        var id = $('#modelId').val();
        $.ajax({
            type: "POST",
            url: "/Home/GetChartParams?id=" + id + "&type=negative",
            dataType: "json",
            success: drawNegativeChart
        });
    });

    $('#showChartForFilter').on('click', function () {
        $(this).hide();
        var id = $('#modelId').val();
        $.ajax({
            type: "POST",
            url: "/Home/GetChartParams?id=" + id + "&type=filtered",
            dataType: "json",
            success: drawFilteredChart
        });
    });   
});