//Libraries
var https = require('https');
var fs = require('fs');
var path = require('path');   

//Globals
var configFile = path.join(__dirname, 'config.json');

////////////
//Quick Sort functions
///////////
function quickSort(arr, left, right, indexString){
  var len = arr.length, 
  pivot,
  partitionIndex;

  if(left < right){
    pivot = right;
    partitionIndex = partition(arr, pivot, left, right, indexString);

    //sort left and right
    quickSort(arr, left, partitionIndex - 1, indexString);
    quickSort(arr, partitionIndex + 1, right, indexString);
  }
  return arr;
}

function partition(arr, pivot, left, right, indexString){
  var pivotValue = parseFloat(arr[pivot][indexString]),
  partitionIndex = left;

  for(var i = left; i < right; i++){
    if(parseFloat(arr[i][indexString]) < pivotValue){
      swap(arr, i, partitionIndex);
      partitionIndex++;
    }
  }
  swap(arr, right, partitionIndex);
  return partitionIndex;
}

function swap(arr, i, j){
  var temp = arr[i];
  arr[i] = arr[j];
  arr[j] = temp;
}
////////////
//End of Quick Sort functions
///////////

//Read in config.json asynchronously
function getConfigs(callback){
  console.log("Entering Function - getConfigs");

  fs.readFile(configFile, {encoding: 'utf-8'}, function(err,data){
    if (!err) {
        callback(JSON.parse(data));
    } else {
        callback(false);
    }
  });
}

//Makes a GET request to CoinMarketCap APIs
function getDataFromCoinMarketCap(configObj, callback){
  console.log("Entering Function - getDataFromCoinMarketCap");

  // CoinMarketCap Get Options
  var coinmarketcap_get_options = {
    host: 'api.coinmarketcap.com',
    path: '/v1/ticker/?limit=' + configObj.apiReturnLimit,
    method: 'GET'
  };

  // CoinMarketCap Get Request
  https.get(coinmarketcap_get_options, function(coinmarketcap_resp){
    var coinmarketcapData = '';

    coinmarketcap_resp.on('error', function(e) {
        callback(false);
    });

    coinmarketcap_resp.on('data', function(chunk)  {
        coinmarketcapData += chunk;
    });

    coinmarketcap_resp.on('end', function() {
        callback(JSON.parse(coinmarketcapData));
    });
  }); 
}

//Makes a POST request to Discord's Webhook APIs
function sendDataToDiscord(configObj, discordMessage, callback){
  console.log("Entering Function - sendDataToDiscord");

  var discord_post_data = JSON.stringify({"file":"","content": discordMessage,"embeds": []});

  // Discord Post Options
  var discord_post_options = {
    host: 'discordapp.com',
    path: '/api/webhooks/' + configObj.webhookID + '/' + configObj.webhookToken,
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Content-Length': discord_post_data.length
    }
  };

  // Discord Post Request
  var post_request = https.request(discord_post_options, function(discord_resp) {
    var discordData = '';

    discord_resp.on('data', function(chunk)  {
      discordData += chunk;
    });

    discord_resp.on('end', function() {
      callback(null, true);
    });

    discord_resp.on('error', function(e) {
      callback(null, false);
    });
  });

  //Execute Discord Post Request
  post_request.write(discord_post_data);
  post_request.end();
}

//Constructs the Discord message to be posted
function constructDiscordMessage(configObj, coinmarketcapJSON){
  console.log("Entering Function - constructDiscordMessage");

  var message = '';
  var messageArray = [];
  messageArray.push("Time for the Top " + configObj.apiReturnLimit + " Crypto Update!");

  if(configObj.show1hInfo){
    var topFive = quickSort(coinmarketcapJSON, 0, coinmarketcapJSON.length - 1, "percent_change_1h").reverse();
    messageArray.push("Top Five One Hour Gainers: \n1. " 
      + topFive[0].name + " (" + topFive[0].percent_change_1h + "%)\n2. " 
      + topFive[1].name + " (" + topFive[1].percent_change_1h + "%)\n3. " 
      + topFive[2].name + " (" + topFive[2].percent_change_1h + "%)\n4. " 
      + topFive[3].name + " (" + topFive[3].percent_change_1h + "%)\n5. "
      + topFive[4].name + " (" + topFive[4].percent_change_1h + "%)");
  };
  if(configObj.show24hInfo){
    var topFive = quickSort(coinmarketcapJSON, 0, coinmarketcapJSON.length - 1, "percent_change_24h").reverse();
    messageArray.push("Top Five 24 Hour Gainers: \n1. " 
      + topFive[0].name + " (" + topFive[0].percent_change_24h + "%)\n2. " 
      + topFive[1].name + " (" + topFive[1].percent_change_24h + "%)\n3. " 
      + topFive[2].name + " (" + topFive[2].percent_change_24h + "%)\n4. " 
      + topFive[3].name + " (" + topFive[3].percent_change_24h + "%)\n5. "
      + topFive[4].name + " (" + topFive[4].percent_change_24h + "%)");
  };

  if(configObj.showCoinsOfInterest && configObj.coinsOfInterest.length > 0){
    var interestMessage = 'Coins Of Interest 24 Hour Gains:';
    for (i = 0; i < coinmarketcapJSON.length; i++){
      if(configObj.coinsOfInterest.indexOf(coinmarketcapJSON[i].name) > -1){
        interestMessage = interestMessage.concat("\n- " + coinmarketcapJSON[i].name + " (" + coinmarketcapJSON[i].percent_change_24h + "%)");
      };
    };
    messageArray.push(interestMessage);
  };

  for (var i = 0, len = messageArray.length; i < len; i++) {
    message = message.concat(messageArray[i] + "\n\n");
  };

  return message;
}

//Main
exports.handler = (event, context, callback) => {
  console.log("Entering Function - main")

  getConfigs(function(configObj){
    if(!configObj) {callback(null, "Error parsing config file.")}; //Config Object Check

    getDataFromCoinMarketCap(configObj, function(coinmarketcapJSON){
      if(!coinmarketcapJSON) {callback(null, "Error making get request to CoinMarketCap.")};  //CoinMarketCap object check

      var message = constructDiscordMessage(configObj, coinmarketcapJSON);
      
      if(!configObj.isTesting){
        sendDataToDiscord(configObj, message, function(isSuccess){
          if(isSuccess){
            callback(null, true);
          }else{
            callback(null, false);
          };
        });
      }else {
          callback(null, message)
      };
    });
  });
};