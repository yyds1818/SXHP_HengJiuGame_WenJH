function FormDateStringFun(dateString) {
    // 非空判断
    if (dateString == null) {
        return "";
    }
    // 取时间戳中实际的值
    var datestr = dateString.substring(6, dateString.length - 2);
    /*console.log(datestr);*/
    // 转换成日期形式
    var date = new Date(parseInt(datestr));
    return date;
}