      

    


app.controller("loginController", function ($scope, $rootScope, $state, $window, $timeout, $cordovaSQLite, $http,$localStorage,ModalService) {
          $rootScope.currentScreen = "login";
          
   $scope.loadingGifDisplay={"display":"block"};
          
          
$rootScope.$storage = $localStorage.$default({
              retailerId: "",
    agentId:"",
    userId:"",
    scanLat:"",
    scanLong:"",
    AgentEmail:""
    
          });
                    
/*$timeout(function(){
    
   if($rootScope.netState=="No network connection"){
               console.log("offline");
              $rootScope.internetAvail=false;
              if($rootScope.$storage.userId==""){
                  //do nothing
                  console.log("user if is blank");
              }
              else if($rootScope.$storage.userId!=""){
                  console.log("user id is not blnk");
              $state.go('landing');}
              
          }
          else{
              console.log(" in online online");
              $rootScope.internetAvail=true;
              
          }
         
    
},2000);*/
    $timeout(function(){
        
        $scope.loadingGifDisplay={"display":"none"};
       if($rootScope.checkConnection()==true){
           
            console.log(" in online online");
             // $rootScope.internetAvail=true;
       } 
    else{
         console.log("offline");
             // $rootScope.internetAvail=false;
              if($rootScope.$storage.userId==""){
                  //do nothing
                  console.log("user id is blank");
              }
              else if($rootScope.$storage.userId!=""){
                  console.log("user id is not blnk");
              $state.go('landing');}
        
    }
    },4000);
    
    
          var onDeviceReady = function () {
              
       $rootScope.baseURL="http://proscanmobile.azurewebsites.net/" ;
$rootScope.checkConnection=function(){
    
   var networkState = navigator.connection.type;

    var states = {};
    states[Connection.UNKNOWN]  = 'Unknown connection';
    states[Connection.ETHERNET] = 'Ethernet connection';
    states[Connection.WIFI]     = 'WiFi connection';
    states[Connection.CELL_2G]  = 'Cell 2G connection';
    states[Connection.CELL_3G]  = 'Cell 3G connection';
    states[Connection.CELL_4G]  = 'Cell 4G connection';
    states[Connection.CELL]     = 'Cell generic connection';
    states[Connection.NONE]     = 'No network connection';

                  var netState=states[networkState];
  //  console.log('Connection type: ' + states[networkState]);
    
    
    if(netState=="No network connection")
        {
           // console.log("returning false for net");
    return false;}
    
    else {
       // console.log("returning true fr net");
        return true;}
}   



  /*$rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
    //var requireLogin = toState.data.requireLogin;

   /* if (requireLogin && typeof $rootScope.currentUser === 'undefined') {
      event.preventDefault();
      // get me a login modal!
    }*/
      
    /*    console.log("in statchangestart");
    if(toState.name === "landing"){
       return;
    }

    if($rootScope.checkConnection()==false) {

        
        console.log("going to landing");
        event.preventDefault();
        $state.transitionTo("landing", null, {notify:false});
        $state.go('landing');
    }
  });*/








  /*            $rootScope.checkConnection=function() {
    var networkState = navigator.connection.type;

    var states = {};
    states[Connection.UNKNOWN]  = 'Unknown connection';
    states[Connection.ETHERNET] = 'Ethernet connection';
    states[Connection.WIFI]     = 'WiFi connection';
    states[Connection.CELL_2G]  = 'Cell 2G connection';
    states[Connection.CELL_3G]  = 'Cell 3G connection';
    states[Connection.CELL_4G]  = 'Cell 4G connection';
    states[Connection.CELL]     = 'Cell generic connection';
    states[Connection.NONE]     = 'No network connection';

                  $rootScope.netState=states[networkState];
    console.log('Connection type: ' + states[networkState]);
}*/
             


              
   
            //  console.log("on device ready");
              /*var client = new WindowsAzure.MobileServiceClient('http://proscanmobiletest.azurewebsites.net/', '74aff4b6-1e45-4998-ac44-27b151045544');*/
var client = new WindowsAzure.MobileServiceClient($rootScope.baseURL, '74aff4b6-1e45-4998-ac44-27b151045544');

              
              //74aff4b6-1e45-4998-ac44-27b151045544- for production
              
              //  f1e4e582-9160-41f1-9d43-3901811b730a  - for test
              $rootScope.db = sqlitePlugin.openDatabase({
                  name: 'syngentaDB.db',
                  location: 'default'
              });
            //  console.log("db is " + $rootScope.db);
              
              
              $rootScope.logOutFunction=function(){
                  console.log("rootscope logout");
                  
                  client.logout();
                  
                  $http.get('https://login.microsoftonline.com/common/oauth2/logout').success(function(data,status,headers,config){
                      
                     console.log("success data "+data); 
                       navigator.app.exitApp();
                  }).error(function(data,status,headers,config){
                      
                      console.log("error "+data);
                  });
                 
              }
                 
              $rootScope.db.transaction(function (tx) {
                          tx.executeSql('CREATE TABLE IF NOT EXISTS ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail)');


                        



                      }, function (error) {

                          console.log('Transaction ERROR: ' + error.message);
                      },
                      function () {
                          console.log('created table OK');                    

                      });
              

              $scope.callAuthMe = function (successCallback, failCallback) {

                  console.log("i am call");
                  var appUrl=$rootScope.baseURL;
                 // var appUrl = "http://proscanmobiletest.azurewebsites.net/";
                  var req = new XMLHttpRequest();
                  req.open("GET", appUrl + "/.auth/me", true);

                  // Here's the secret sauce: X-ZUMO-AUTH
                  req.setRequestHeader('X-ZUMO-AUTH', client.currentUser.mobileServiceAuthenticationToken);

                  req.onload = function (e) {
                      if (e.target.status >= 200 && e.target.status < 300) {
                          successCallback(JSON.parse(e.target.response));
                          return;
                      }
                     failCallback('Data request failed. Status ' + e.target.status + ' ' + e.target.response);
                  };

                  req.onerror = function (e) {
                      failCallback('Data request failed: ' + e.error);
                  }

                  req.send();
              }

              $scope.getUserId = function (a) {
                  console.log(a);
                   $scope.url = $rootScope.baseURL+"api/User?email=" + a;
                 // $scope.url = "https://proscanmobiletest.azurewebsites.net/api/User?email=" + a;
                  $rootScope.onUserIdAuthenticationSuccess = function (data, status, headers, config) {
                      console.log(data);
                      $rootScope.retailerId = data.RetailerId;
                      $rootScope.agentId = data.AgentId;
                      $rootScope.role = data.Role;
                      console.log($rootScope.retailerId);
                      console.log($rootScope.agentId);
                      console.log($rootScope.role);

                      
                      
                      
                      
                      
                    if($rootScope.role=="R")
                      {
                          console.log("in R");
                           ModalService.showModal({
            templateUrl: 'templates/dialogs/retailerDialog.html',
            controller: "ModalController"
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                
            });
        });
                         // $state.go('landing');
                      }
                      else{
                          console.log("in A");
                            ModalService.showModal({
            templateUrl: 'templates/dialogs/agentDialog.html',
            controller: "ModalController"
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                
            });
        });
                         // $state.go('landing');
                      }

                      
               
                      $http.get($rootScope.baseURL+'api/Register?loginId=' + $rootScope.userId + '&type=' + $rootScope.role + '&handle=' + $rootScope.handle).success(function (data, status, headers, config) {
                       /*console.log('https://proscanmobiletest.azurewebsites.net/api/Register?loginId=' + $rootScope.userId + '&type=' + $rootScope.role + '&handle=' + $rootScope.handle);*/
                          console.log("data on reg" + data);
                          
                          console.log("retailer id"+$rootScope.retailerId);
                               
                               //local storage data strore
                      

                          $rootScope.$storage.retailerId=$rootScope.retailerId;
                           $rootScope.$storage.agentId=$rootScope.agentId;
                           $rootScope.$storage.userId=$rootScope.userId;
                          
                          console.log("$rootScope.$storage.retailerId"+$rootScope.$storage.retailerId);
                          
                          
                      }).error(function (data, status, headers, config) {
                          console.log("data on error" + data);
                      })




                  }
                  $rootScope.onUserIdAuthenticationError = function (data, status, headers, config) {
                      console.log("error");
                  }
                  $http.get($scope.url).success($rootScope.onUserIdAuthenticationSuccess).error($rootScope.onUserIdAuthenticationError);
              }

              $scope.googleSignIn = function () {
                  console.log("im insert");
                  client.login('google').done(function (results) {
                      console.log("results" + Object.keys(results));
                      //$rootScope.userLoggedIn=1;
                      $rootScope.googleUserLoggedIn=1;
                      
                      $state.go('landing');
                  }, function (err) {
                      console.log("fail");
                  });
              }
              $scope.onADLogin = function () {
                  console.log("im select");
                  client.login('aad').done(function (results) {
                          console.log(results);

                          $scope.callAuthMe(function (result) {
                              $rootScope.userLoggedIn=1;
                                  $state.go('landing');

                                  $rootScope.userId = result[0]["user_id"];
                                  console.log($rootScope.userId);
                                  $scope.getUserId($rootScope.userId);

                              },

                              function (msg) {
                                  console.error(msg);
                              });
                      },
                      function (err) {
                          console.log("fail");
                      });
                 
                  
              }



             // var client1 = new WindowsAzure.MobileServiceClient("https://proscanmobiletest.azurewebsites.net");
               var client1 = new WindowsAzure.MobileServiceClient($rootScope.baseURL);
              // Register for Push Notifications. Requires that phonegap-plugin-push be installed.

              var pushRegistration = null;

              function registerForPushNotifications() {
                  console.log("registering for push notifctaion");
                  pushRegistration = PushNotification.init({
                      android: {
                          senderID: '228029623805'
                      },
                      ios: {
                          alert: 'true',
                          badge: 'true',
                          sound: 'true'
                      },
                      wns: {}
                  });

                  // Handle the registration event.
                  pushRegistration.on('registration', function (data) {
                      //console.log("data "+data);
                      console.log("in on registration");
                      // Get the native platform of the device.
                      var platform = device.platform;
                      // Get the handle returned during registration.
                      var handle = data.registrationId;
                      // Set the device-specific message template.
                      $rootScope.handle = handle;
                      console.log("handle" + handle);
                      if (platform == 'android' || platform == 'Android') {
                          // Register for GCM notifications.
                          // console.log("in platform=android");
                          // console.log("message param " + $(messageParam));
                          client1.push.register('gcm', handle, {
                              mytemplate: {
                                  body: {
                                      data: {
                                          message: "{$(messageParam)}"
                                      }
                                  }
                              }
                          });
                      } else if (device.platform === 'iOS') {
                          // Register for notifications.            
                          client1.push.register('apns', handle, {
                              mytemplate: {
                                  body: {
                                      aps: {
                                          alert: "{$(messageParam)}"
                                      }
                                  }
                              }
                          });
                      } else if (device.platform === 'windows') {
                          // Register for WNS notifications.
                          client1.push.register('wns', handle, {
                              myTemplate: {
                                  body: '<toast><visual><binding template="ToastText01"><text id="1">$(messageParam)</text></binding></visual></toast>',
                                  headers: {
                                      'X-WNS-Type': 'wns/toast'
                                  }
                              }
                          });
                      }
                  });

                  pushRegistration.on('notification', function (data, d2) {
                      alert('Notification Received: ' + data.message);
                      console.log('Push Received: ' + data.message);
                     // $rootScope.getAllDataFromServer();
                      
                  });

                  function handleError() {

                      console.log("error handled");
                  }
                  pushRegistration.on('error', handleError);
              }


              registerForPushNotifications();




          }




          document.addEventListener("deviceready", onDeviceReady, false);




          function onBackKeyDown() {
              // Handle the back button
              if ($rootScope.currentScreen == "productValidationScreen") {
                  $state.go('scanOptions');
              }
              if ($rootScope.currentScreen == "scanOptions") {
                  $state.go('landing');
              }
              if ($rootScope.currentScreen == "landing") {
                //  $state.go('login');
                   navigator.app.exitApp();
              }
              if ($rootScope.currentScreen == "login") {
                  console.log("curr login");
                  // event.preventDefault(); // EDIT

                  navigator.app.exitApp();
              }
              if ($rootScope.currentScreen == "productScanHistoryScreen") {
                  $state.go('landing');

              }

          }
          document.addEventListener("backbutton", onBackKeyDown, false);




      });

      app.controller('landingController', function ($scope, $rootScope, $state, $timeout, $http,$localStorage,ModalService)

          {
          
          $scope.myScansClicked=function(){
              $state.go('productScanHistory');
          }
          
          
          $scope.proscanClicked=function(){
              $state.go('scanOptions');
              
          }
          $scope.logoutClicked=function(){
              console.log("logout clicked");
              if($rootScope.userLoggedIn==1)
            $rootScope.logOutFunction();
              else{
                  $http.get('https://mail.google.com/mail/u/0/?logout&hl=en').success(function(data,status,headers,config){
                     console.log("hit service");
                      navigator.app.exitApp();
                      
                  }).error(function(data,status,headers,config){
                      console.log("error");
                  });
              }
          }
          
        var menuflag = 0;
//   $scope.menuSearchBtnStyle = { "background-color": "#2F434E" };
    $scope.menuOpen = function () {
console.log("menu open");
        if (menuflag == 0) {
            angular.element(document.querySelector('.main-nav')).addClass('openSideBar');
            angular.element(document.querySelector('.proScanMenuPageHeader')).addClass('movePatientSearchScreen');
            angular.element(document.querySelector('.menuItemsContainer')).addClass('movePatientSearchScreen');
           angular.element(document.querySelector('.landingPageFooter')).addClass('movePatientSearchScreen');
            menuflag = 1;
        }
        else {
            angular.element(document.querySelector('.main-nav')).removeClass('openSideBar');
            angular.element(document.querySelector('.proScanMenuPageHeader')).removeClass('movePatientSearchScreen');
            angular.element(document.querySelector('.menuItemsContainer')).removeClass('movePatientSearchScreen');
          angular.element(document.querySelector('.landingPageFooter')).removeClass('movePatientSearchScreen');
            menuflag = 0;
        }


    }

          if($rootScope.$storage.retailerId!=""){
              
              $rootScope.retailerId=$rootScope.$storage.retailerId;
          
$rootScope.agentId=$rootScope.$storage.agentId;
              
             
          }
      console.log(" $rootScope.retailerId"+ $rootScope.$storage.retailerId+"\n$rootScope.agentId"+$rootScope.$storage.agentId);
              

              $rootScope.multipleScanItems = [];
              $rootScope.mul = [];
              $rootScope.currentScreen = "landing";
              console.log("landing Page");



              // var client1 = new WindowsAzure.MobileServiceClient("https://proscantest.azurewebsites.net");




              $scope.onProscanClicked = function () {

                  $state.go('scanOptions');
              }
              $scope.onMyScansClicked = function () {
                  $state.go('productScanHistory');
              }
              var a = navigator.onLine;
              if (a == true) {
}

          });
      app.controller('registrationController', function ($scope, $rootScope, $state) {

          $scope.onRegistrationClicked = function () {

              console.log("registration clicked");
          }

      });
      app.controller("prodScanResultController", function ($scope, $rootScope, $state) {
          $scope.onCancelClicked = function () {
              $state.go('landing');
          }
          $scope.onValidateClicked = function () {
              console.log("on validate clicked");
              $state.go('productValidationResult');
          }

      });




      app.controller("productScanCurrentListController", function ($scope, $rootScope, $state, $timeout) {

          
          console.log("scan list controller");


          $scope.multipleScanItems = [];


          var readFromTable = function () {
             console.log("reading in product scan list");
              $rootScope.db.transaction(function (tx) {


                  //var str='select * from scanresults where isValid="'+true+'" and isProcessed="

                  tx.executeSql('SELECT * FROM ScanResults WHERE syncStatus=' + 0 + '', [], function (tx, results) {

                      var len = results.rows.length,
                          i;
                      console.log("len" + len);
                      for (i = 0; i < len; i++) {

                          // console.log(i + " th record n pendingresults where isProcessed=false" + JSON.stringify(results.rows.item(i)));
                          $scope.multipleScanItems.push(results.rows.item(i));
                          $scope.$apply();

                      }


                  });



              });

              console.log("$scope.multipleScanItems" + $scope.multipleScanItems);
              console.log("$scope.multipleScanItems data" + JSON.stringify($scope.multipleScanItems[0]));

          }

          readFromTable();


          $scope.onHistoryIconClicked = function () {

              $state.go('productScanHistory');
          }
          $scope.singleScanImageSrc = "assets/images/slices/u940.png";
          $scope.tickIcon = "assets/images/slices/u1047.png";



          $scope.onBackClicked = function () {

              $state.go('landing');
          }
          
          
          
          
          // var networkAvailability = navigator.onLine;
          if ($rootScope.checkConnection() == false) {
              $rootScope.single = 0;
              $rootScope.scanOptionSelected=true;
              $scope.singleScanStyleClass = {
              "border-color": "rgba(221, 221, 221, 1)"
          };
              $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
              
          }
          else{
              $rootScope.single = 0;
              $rootScope.scanOptionSelected=true;
             $scope.singleScanStyleClass = {
              "border-color": "#999999"
          };  
              
               $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
              
          }
          
          
          
          $scope.singleScanClicked = function () {
              // var network = navigator.onLine;
          if ($rootScope.checkConnection() == true) {
              // console.log("oline");

              $rootScope.single = 1;
              $rootScope.scanOptionSelected = true;
              $scope.singleScanStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
              // $scope.singleScanImageSrc="assets/images/slices/u7008.png";
              $scope.tickIcon = "assets/images/slices/u443.png";
              $scope.sqStyleClass = {
                  "border-color": "#999999"
              };
              $scope.manualBarcode = "";

          } 
             
          }
          $scope.tickIcon = "assets/images/slices/u443.png";
          $scope.sqStyleClass = {
              "border-color": "rgba(14, 180, 79, 1)"
          };
          $scope.multipleScanClicked = function () {
              $rootScope.single = 0;
              $rootScope.scanOptionSelected = true;
             if($rootScope.checkConnection()==true){
              $scope.singleScanStyleClass = {
                  "border-color": "#999999"
              };}
              $scope.tickIcon = "assets/images/slices/u443.png";
              $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
              $scope.manualBarcode = "";

          }
       
          $scope.checkBarcode = function () {
              
              if($rootScope.checkConnection()==false){
                   if ($scope.manualBarcode!=null) {
                  
                $rootScope.manualBarcodescan = 0;
                      $rootScope.scanOptionSelected = false;
                      $scope.tickIcon = "assets/images/slices/u1047.png";  
                   }
                  else{
                      $rootScope.manualBarcodescan = 0;
                       $rootScope.single = 0;
              $rootScope.scanOptionSelected = true;
                       $scope.tickIcon = "assets/images/slices/u443.png";
              $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
                  }
              }
              else{
              if ($scope.manualBarcode!=null) {
                  $rootScope.scanOptionSelected = false;
                  $scope.tickIcon = "assets/images/slices/u1047.png";
                  $scope.sqStyleClass = {
                      "border-color": "#999999"
                  };
                  $scope.singleScanStyleClass = {
                      "border-color": "#999999"
                  };

                  var barcodeValidity = Barcoder.validate($scope.manualBarcode.toString());

                  if (barcodeValidity == true) {

                      // console.log("true");
                      $rootScope.scanOptionSelected = false;
                      $scope.tickIcon = "assets/images/slices/u443.png";
                      $rootScope.scannedValue = $scope.manualBarcode;
                      $rootScope.manualBarcodescan = 1;

                  } else {
                      $rootScope.manualBarcodescan = 0;
                      $rootScope.scanOptionSelected = false;
                      $scope.tickIcon = "assets/images/slices/u1047.png";
                      $scope.sqStyleClass = {
                          "border-color": "#999999"
                      };
                      $scope.singleScanStyleClass = {
                          "border-color": "#999999"
                      };

                  }
              } else {
                  $rootScope.manualBarcodescan = 0;
                  console.log("thsi s blank");
                  $scope.tickIcon = "assets/images/slices/u443.png";

                  $scope.singleScanStyleClass = {
                      "border-color": "rgba(14, 180, 79, 1)"
                  };
                  $scope.sqStyleClass = {
                      "border-color": "#999999"
                  };
                  $rootScope.scanOptionSelected = true;
                  $scope.single = 1;
              }
          }
          }



      });




      app.controller("scanOptionsController", function ($scope, $rootScope, $state, $timeout, $q, multipleScanItems, $localStorage,ModalService) {
          $rootScope.$storage = $localStorage.$default({
              BatchId: 1
          });
          console.log("$rootScope.$storage.BatchId" + $rootScope.$storage.BatchId);
          $scope.onHistoryIconClicked = function () {

              $state.go('productScanHistory');
          }


          $rootScope.single = 1;
          $rootScope.manualBarcodescan = 0;
          $rootScope.currentScreen = "scanOptions";
          //  $scope.singleScanImageSrc = "assets/images/slices/u940.png";
          $scope.tickIcon = "assets/images/slices/u443.png";
          
          
          
          
         //  var networkAvailability = navigator.onLine;
          if ($rootScope.checkConnection() == false) {
              $rootScope.single = 0;
              $rootScope.scanOptionSelected=true;
              $scope.singleScanStyleClass = {
              "border-color": "rgba(221, 221, 221, 1)"
          };
              $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
              
          }
          else{
              
             $scope.singleScanStyleClass = {
              "border-color": "rgba(14, 180, 79, 1)"
          };  
              
               $scope.sqStyleClass = {
                  "border-color": "#999999"
              };
              
          }
          
          $rootScope.scanOptionSelected = true;

          $scope.onBackClicked = function () {
              $state.go('landing');
          }


          var readFromTable = function () {
              console.log("reading");
              $rootScope.db.transaction(function (tx) {




                  tx.executeSql('SELECT * FROM ScanResults', [], function (tx, results) {

                      var len = results.rows.length,
                          i;
                      console.log("len" + len);
                      for (i = 0; i < len; i++) {

                          console.log(i + "th record" + JSON.stringify(results.rows.item(i)));

                      }


                  });


              });



          }

          var insertScanToDB = function (currentTime) {

console.log("in inserttodb");



              var onLocSuccess = function (position) {

                  $rootScope.geoLocLatValues = position.coords.latitude;
                  $rootScope.geoLocLongtValues = position.coords.longitude;
                  console.log("position.coords.latitude" + position.coords.latitude);

                  console.log("position.coords.longitude" + position.coords.longitude);








                  $rootScope.db.transaction(function (tx) {
                      console.log("started db insert");
                         /* tx.executeSql('CREATE TABLE IF NOT EXISTS ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail)');
*/

                          tx.executeSql('INSERT INTO ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail) VALUES ("' + $rootScope.retailerId + '","' + $rootScope.agentId + '","' + $rootScope.scannedValue + '","' + '' + '","' + 'false' + '","' + 'false' + '","' + 'pending' + '","' + currentTime + '","' + $rootScope.geoLocLatValues + '","' + $rootScope.geoLocLongtValues + '",' + $rootScope.$storage.BatchId + ',"' + 'false' + '","' + '' + '","Scanned Item ","assets/images/slices/u1186.png",0'+',"'+$rootScope.$storage.AgentEmail+'")');




                      }, function (error) {

                          console.log('Transaction ERROR: ' + error.message);
                      },
                      function () {
                          console.log('populated database OK');
                          // readFromTable();
                          $state.go('productCurrentScanList', {}, {
                              reload: true
                          });

                      });




              };

              function onLocError(error) {
                  console.log('code: ' + error.code + '\n' +
                      'message: ' + error.message + '\n');
                  
                  
                  
              }
          //  var networkAvailability = navigator.onLine;
          if ($rootScope.checkConnection() == true) {
              navigator.geolocation.getCurrentPosition(onLocSuccess, onLocError,{
  enableHighAccuracy: true,
  timeout: 30000,
  maximumAge: 30000
});

          }
              else{
                  console.log("no network for geolcation");
                  
                  console.log("$rootScope.$storage.scanLat"+$rootScope.$storage.scanLat+"\n$rootScope.$storage.scanLong"+$rootScope.$storage.scanLong);
                  
                     $rootScope.db.transaction(function (tx) {
                      console.log("started db insert");
                         /* tx.executeSql('CREATE TABLE IF NOT EXISTS ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail)');
*/
                     if( $rootScope.$storage.scanLat=="" && $rootScope.$storage.scanLong==""){
                         console.log("both are blank");
                          tx.executeSql('INSERT INTO ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail) VALUES ("' + $rootScope.retailerId + '","' + $rootScope.agentId + '","' + $rootScope.scannedValue + '","' + '' + '","' + 'false' + '","' + 'false' + '","' + 'pending' + '","' + currentTime + '","' + '51.28'+ '","' + '0.0' + '",' + $rootScope.$storage.BatchId + ',"' + 'false' + '","' + '' + '","Scanned Item ","assets/images/slices/u1186.png",0'+',"'+$rootScope.$storage.AgentEmail+'")');

                     }
                         else{
                             console.log("either one or both r not blank");
                           tx.executeSql('INSERT INTO ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail) VALUES ("' + $rootScope.retailerId + '","' + $rootScope.agentId + '","' + $rootScope.scannedValue + '","' + '' + '","' + 'false' + '","' + 'false' + '","' + 'pending' + '","' + currentTime + '","' +$rootScope.$storage.scanLat + '","' + $rootScope.$storage.scanLong + '",' + $rootScope.$storage.BatchId + ',"' + 'false' + '","' + '' + '","Scanned Item ","assets/images/slices/u1186.png",0'+',"'+$rootScope.$storage.AgentEmail+'")');  
                             
                         }


                      }, function (error) {

                          console.log('Transaction ERROR: ' + error.message);
                      },
                      function () {
                          console.log('populated database OK');
                          // readFromTable();
                          $state.go('productCurrentScanList', {}, {
                              reload: true
                          });

                      });
                  
              }

          }
          $rootScope.scan = function () {
              if ($rootScope.scanOptionSelected == true) {
                  cordova.plugins.barcodeScanner.scan(
                      function (result) {

                          if (result.cancelled == true) {
                              $state.go('scanOptions');
                          } else {


                              
                              
                              
                              
                              
                              console.log("result " + Object.keys(result));
                              console.log("result.cancelled" + result.cancelled);
                              console.log("scanned data" + result.text);
                              $rootScope.scannedValue = result.text;
                              
                              
                              /*if(!$rootScope.scannedValue.match(/^\d+$/)){
                                  
                                  console.log("Valid barcode should contain only digits. Please try again");
                              }
                              else{*/

                              if ($rootScope.single == 1) {
                                  console.log("single==1");
                                  $state.go('productValidationResult');

                              } else {
                                  console.log("in else bloc");

                                  var today = new Date();
                                  var dd = today.getUTCDate();
                                  var mm = today.getUTCMonth() + 1; //January is 0!

                                  var yyyy = today.getUTCFullYear();
                                  if (dd < 10) {
                                      dd = '0' + dd
                                  }
                                  if (mm < 10) {
                                      mm = '0' + mm
                                  }
                                  var dateInFormat = yyyy + '-' + mm + '-' + dd;
                                  var minute = today.getUTCMinutes();
                                  if (minute < 10) {
                                      minute = '0' + minute;
                                  }
                                  var hour = today.getUTCHours();
                                  if (hour < 10) {
                                      hour = '0' + hour;
                                  }
                                  $scope.currentTime = hour + ":" + minute;
                                  $scope.currentDate = dateInFormat;
                                  //console.log("pushing into array");
                                  // $rootScope.multipleScanItems= multipleScanItems;
                                  /*$rootScope. multipleScanItems.push({
                                            "productName":"Item",
                                            "barcodeNumber":$rootScope.scannedValue,
                                            "captureTime":$scope.currentDate+" "+$scope.currentTime
                                            
                                        });*/


                                  $scope.currentDateTime = $scope.currentDate + "T" + $scope.currentTime;
console.log("going to insert to db");
                                  console.log(" $scope.currentDateTime"+ $scope.currentDateTime);
                                  insertScanToDB($scope.currentDateTime);

                                  // console.log("array is" + $rootScope.multipleScanItems + "\narray lnght" + $rootScope.multipleScanItems.length);
                                  /*$timeout(function() {
                                      $state.go('productCurrentScanList');
                                  }, 300);*/
                                  //   $state.go('productCurrentScanList');             
                              }
                              
                         // }
                              
                          }

                      },


                      function (error) {
                          alert("Scanning failed: " + error);
                      }
                  )
              } else {
                  if ($rootScope.manualBarcodescan == 1)
                      $state.go('productValidationResult');
              }
          }
          $scope.singleScanClicked = function () {
              if ($rootScope.checkConnection() == true) {
              
              $rootScope.single = 1;
              $rootScope.scanOptionSelected = true;
              $scope.singleScanStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };

              $scope.tickIcon = "assets/images/slices/u443.png";
              $scope.sqStyleClass = {
                  "border-color": "#999999"
              };
              $scope.manualBarcode = "";
              }
          }
          $scope.multipleScanClicked = function () {
              $rootScope.single = 0;
              $rootScope.scanOptionSelected = true;
              if($rootScope.checkConnection()==true){
              $scope.singleScanStyleClass = {
                  "border-color": "#999999"
              };}
              $scope.tickIcon = "assets/images/slices/u443.png";
              $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
              $scope.manualBarcode = "";
          }

          $scope.checkBarcode = function () {
              
              if($rootScope.checkConnection()==false){
                   if ($scope.manualBarcode!=null) {
                  
                $rootScope.manualBarcodescan = 0;
                      $rootScope.scanOptionSelected = false;
                      $scope.tickIcon = "assets/images/slices/u1047.png";  
                   }
                  else{
                      $rootScope.manualBarcodescan = 0;
                       $rootScope.single = 0;
              $rootScope.scanOptionSelected = true;
                       $scope.tickIcon = "assets/images/slices/u443.png";
              $scope.sqStyleClass = {
                  "border-color": "rgba(14, 180, 79, 1)"
              };
                  }
              }
              else{
              if ($scope.manualBarcode!=null) {
                  $rootScope.scanOptionSelected = false;
                  $scope.tickIcon = "assets/images/slices/u1047.png";
                  $scope.sqStyleClass = {
                      "border-color": "#999999"
                  };
                  $scope.singleScanStyleClass = {
                      "border-color": "#999999"
                  };

                  var barcodeValidity = Barcoder.validate($scope.manualBarcode.toString());

                  if (barcodeValidity == true) {

                      // console.log("true");
                      $rootScope.scanOptionSelected = false;
                      $scope.tickIcon = "assets/images/slices/u443.png";
                      $rootScope.scannedValue = $scope.manualBarcode;
                      $rootScope.manualBarcodescan = 1;

                  } else {
                      $rootScope.manualBarcodescan = 0;
                      $rootScope.scanOptionSelected = false;
                      $scope.tickIcon = "assets/images/slices/u1047.png";
                      $scope.sqStyleClass = {
                          "border-color": "#999999"
                      };
                      $scope.singleScanStyleClass = {
                          "border-color": "#999999"
                      };

                  }
              } else {
                  $rootScope.manualBarcodescan = 0;
                  console.log("thsi s blank");
                  $scope.tickIcon = "assets/images/slices/u443.png";

                  $scope.singleScanStyleClass = {
                      "border-color": "rgba(14, 180, 79, 1)"
                  };
                  $scope.sqStyleClass = {
                      "border-color": "#999999"
                  };
                  $rootScope.scanOptionSelected = true;
                  $scope.single = 1;
              }
          }
          }

      });




      app.controller("productScanHistoryController", function ($scope, $rootScope, $state, $http, $timeout,ModalService,$localStorage) {
         
                      $scope.verified=1;
         
$scope.onShareClicked = function(data) {
    
    
    
    if($rootScope.$storage.AgentEmail=="")
        {
           alert('Please sync to get the Agent email address and then share'); 
            
        }
    else{
              console.log("data pulled " + data.ProductName);
              $scope.email = "kevin.osullivan@hpe.com";
               if(data.ProductName == "null" || data.ProductName == null || data.ProductName == "" ||data.ProductName == 'undefined'){
                 document.location.href = 'mailto:' + $rootScope.$storage.AgentEmail + '?subject=Re: Invalid Product'+ '&body=Barcode Value:' + data.BarcodeValue;
               }
               else{
                document.location.href = 'mailto:' + $rootScope.$storage.AgentEmail + '?subject=Re: ' + data.ProductName + '&body=Product Name:' + data.ProductName + '%0D%0A' + 'Product ID:' + data.ProductID+ '%0D%0A' + 'Barcode Value:' + data.BarcodeValue;                
}
          
    }


}
             
          
          $rootScope.currentScreen = "productScanHistoryScreen";
          console.log("in product scan history");
          $scope.onPullSuccess = function (data, status, headers, config) {
              console.log("data pulled " + data.Result.length);

              $scope.loadingGifDisplay.display = "none";
              if(data.Result){
                  $rootScope.$storage.AgentEmail= data.Result[0].AgentEmail;                  
                  
              }
              
              insertIntoTable(data);

          }
        var sortAccordingToDate=function(){
            console.log("in sort");
                         if((document.getElementById('startDate').value)||(document.getElementById('endDate').value)){
                          
                          $scope.tempScannedHistory = angular.copy($scope.scannedHistory);
                      

                      console.log("$scope.tempScannedHistory" + $scope.tempScannedHistory.length);
                      $scope.scannedHistory = [];
                          
                      //  (new Date($scope.tempScannedHistory[i].ScanDate.split('T')[0]).valueOf() <= new Date(document.getElementById("endDate").value).valueOf())  
                           for (var i = 0; i < $scope.tempScannedHistory.length; i++) {
                         
                         
                               
                               if((document.getElementById('startDate').value)&&(document.getElementById('endDate').value)){
                               
                          if ((new Date($scope.tempScannedHistory[i].ScanDate.split('T')[0]).valueOf() >= new Date(document.getElementById("startDate").value).valueOf()) && (new Date($scope.tempScannedHistory[i].ScanDate.split('T')[0]).valueOf() <= new Date(document.getElementById("endDate").value).valueOf()) ) {
                             
                              $scope.scannedHistory.push($scope.tempScannedHistory[i]);
                              $scope.$apply();
                          }
                               
                               }
                               else if((document.getElementById('startDate').value)&&(!(document.getElementById('endDate').value))){
                                   
                                     if ((new Date($scope.tempScannedHistory[i].ScanDate.split('T')[0]).valueOf() >= new Date(document.getElementById("startDate").value).valueOf())  ) {
                             
                              $scope.scannedHistory.push($scope.tempScannedHistory[i]);
                              $scope.$apply();
                          }
                               }
                                   else if( !(document.getElementById('startDate').value)&&((document.getElementById('endDate').value)) ){
                                       
                                             
                          if ( (new Date($scope.tempScannedHistory[i].ScanDate.split('T')[0]).valueOf() <= new Date(document.getElementById("endDate").value).valueOf()) ) {
                             
                              $scope.scannedHistory.push($scope.tempScannedHistory[i]);
                              $scope.$apply();
                          }
                                       
                                   }
                               }
                      }
                      
                      else{
                          
                          $scope.$apply();
                      }
                  
                  
        }
                  
              
          $scope.readVerifiedFromTable = function () {
              console.log("read verified from table");
              
              $scope.verifiedBtnStyle={"border-color":" rgba(0, 160, 190, 1)","color":"white","background-color":"rgba(0, 160, 190, 1)"};
              $scope.pendingBtnStyle={"border-color":" rgba(204, 204, 204, 1)","color":"rgba(204, 204, 204, 1)","background-color":"white"};
              $scope.failedBtnStyle={"border-color":" rgba(204, 204, 204, 1)","color":"rgba(204, 204, 204, 1)","background-color":"white"};
              $scope.pending=0;
              $scope.verified=1;
              $scope.failed=0;
              console.log("verfied clicked");
              $scope.scannedHistory = [];
              console.log("reading");
              
      
              $rootScope.db.transaction(function (tx) {

                  tx.executeSql('SELECT count(*) AS mycount FROM ScanResults', [], function (tx, rs) {
                      console.log('Record count : ' + rs.rows.item(0).mycount);
                  }, function (tx, error) {
                      console.log('SELECT error: ' + error.message);
                  });

                  tx.executeSql('SELECT * FROM ScanResults where isProcessed="' + true + '" AND isValid="' + 'true' + '"', [], function (tx, results) {

                      var len = results.rows.length,
                          i;
                      console.log("len of verfied reocrds" + len);
                      for (i = 0; i < len; i++) {

                           console.log(i + " th record " + JSON.stringify(results.rows.item(i)));
                          $scope.scannedHistory.push(results.rows.item(i));
                         // $scope.$apply();

                      }
                    
                      
                      
                    sortAccordingToDate();
                      
                      console.log("new scanned history length" + $scope.scannedHistory.length + "\n$scope.scannedHistory" + $scope.scannedHistory);
                  });



              });



          }

          $scope.readPendingFromTable = function () {
               $scope.pendingBtnStyle={"border-color":" rgba(0, 160, 190, 1)","color":"white","background-color":"rgba(0, 160, 190, 1)"};
              $scope.verifiedBtnStyle={"border-color":" rgba(204, 204, 204, 1)","color":"rgba(204, 204, 204, 1)","background-color":"white"};
              $scope.failedBtnStyle={"border-color":" rgba(204, 204, 204, 1)","color":"rgba(204, 204, 204, 1)","background-color":"white"};
              $scope.pending=1;
              $scope.verified=0;
              $scope.failed=0;
              console.log("pendng clicked");
              $scope.scannedHistory = [];
              console.log("reading");
              $rootScope.db.transaction(function (tx) {

                  tx.executeSql('SELECT * FROM ScanResults where BatchID IS NOT NULL AND isProcessed="'+false+'" ORDER BY syncStatus ASC ', [], function (tx, results) {

                      var len = results.rows.length,
                          i;
                      console.log("len of pending recors" + len);
                      for (i = 0; i < len; i++) {

                          console.log(i + " th record " + JSON.stringify(results.rows.item(i)));
                          $scope.scannedHistory.push(results.rows.item(i));

                         // $scope.$apply();

                      }

                      console.log("length of scanend histirey" + $scope.scannedHistory.length);
                      sortAccordingToDate();
                  });



              });



          }

          $scope.readFailedFromTable = function () {
               $scope.failedBtnStyle={"border-color":" rgba(0, 160, 190, 1)","color":"white","background-color":"rgba(0, 160, 190, 1)"};
              $scope.pendingBtnStyle={"border-color":" rgba(204, 204, 204, 1)","color":"rgba(204, 204, 204, 1)","background-color":"white"};
              $scope.verifiedBtnStyle={"border-color":" rgba(204, 204, 204, 1)","color":"rgba(204, 204, 204, 1)","background-color":"white"};
              
              $scope.pending=0;
              $scope.verified=0;
              $scope.failed=1;
              console.log("faied clicked");
              $scope.scannedHistory = [];
              console.log("reading");
              $rootScope.db.transaction(function (tx) {


                  tx.executeSql('SELECT * FROM ScanResults where isProcessed="' + true + '" AND isValid="' + 'false' + '"', [], function (tx, results) {

                      var len = results.rows.length,
                          i;
                      console.log("len of failed records" + len);
                      for (i = 0; i < len; i++) {

                          console.log(i + " th record " + JSON.stringify(results.rows.item(i)));
                          $scope.scannedHistory.push(results.rows.item(i));
                        //  $scope.$apply();

                      }

                      console.log("length of scanend histirey" + $scope.scannedHistory.length);
                      sortAccordingToDate();
                  });



              });



          }


          $rootScope.syncToServer = function () {
              
            //  console.log("$rootScope.internetAvail"+$rootScope.internetAvail);
             // var network_availability = navigator.onLine;
          if ($rootScope.checkConnection() == true) {
              console.log("$rootScope.userLoggedIn "+$rootScope.userLoggedIn);
              if($rootScope.userLoggedIn==1)//user had logged in before->he was in online mode since the launch of the app
              {
              console.log("sync to server");
$scope.loadingGifDisplay = {
                          "display": "block"
                      };
              $scope.offlineScannedItems = [];

              var syncRecordCount=0;
              
                  $rootScope.db.transaction(function (tx) {

                  tx.executeSql('SELECT count(*) AS mycount FROM ScanResults where syncStatus=0', [], function (tx, rs) {
                      console.log('Record count : ' + rs.rows.item(0).mycount);
                      syncRecordCount=rs.rows.item(0).mycount;
                  }, function (tx, error) {
                      console.log('SELECT error: ' + error.message);
                  });
               }, function (error) {

                          console.log('Transaction ERROR: ' + error.message);
                      },
                      function () {
                          console.log('read database');
                          // readFromTable();
                      
                      
                   if(syncRecordCount==0){
                      
                       $rootScope.getAllDataFromServer();
                       
                     
                   }
                      else{                          
                          
                             console.log("synrecordcount!=0");
                          
                          $http.get($rootScope.baseURL+'api/retailers/?retailerId='+$rootScope.retailerId).success(function(data,status,headers,config){
                              console.log("data is"+data);
                              
                              $rootScope.BatchId=data;
                                  
                          $rootScope.db.transaction(function(tx){
                              
                              
                              tx.executeSql('UPDATE ScanResults set BatchId='+$rootScope.BatchId+' where syncStatus=0',[],function(tx,results){
                                 
                                  
                                  
                              }, function (tx, error) {
                      console.log('update error: ' + error.message);
                  });
                              
                          },function(error){
                              
                              console.log("error in transaction");
                          },
                           function(){
                                
                               
                                             
              $rootScope.db.transaction(function (tx) {
                      tx.executeSql('SELECT * FROM ScanResults where syncStatus=0 ', [], function (tx, results) {

                          var len = results.rows.length,
                              i;
                          console.log("len of pending recors to sync" + len);
                          for (i = 0; i < len; i++) {

                              console.log(i + " th record " + JSON.stringify(results.rows.item(i)));
                              $scope.offlineScannedItems.push(results.rows.item(i));
                              $scope.$apply();

                          }

                          console.log("length of offlinescanneditems array" + $scope.offlineScannedItems.length);
                      });



                  }, function (error) {

                      console.log('Transaction ERROR: ' + error.message);
                  },
                  function () {
                      console.log('finished reading from pending table');
                      for (var i = 0; i < $scope.offlineScannedItems.length; i++)
                          console.log(JSON.stringify($scope.offlineScannedItems[i]));
                     
                          
                        $http({    
                          url:   'https://proscanmobile.azurewebsites.net/api/User',
                              method:   'POST',
                              contentType:   "application/json",
                              data:  $scope.offlineScannedItems,
                      }).success(function (data, status, headers, config)  {

                               // do something on success 
                          console.log("Data" + data);
                            
                            $rootScope.db.transaction(function (tx) {
                      tx.executeSql('UPDATE ScanResults SET syncStatus=1 where BatchId='+$rootScope.BatchId, [], function (tx, results) {
                          
                      })
                            },function (error) {
                                
                                console.log('Transaction ERROR: ' + error.message);
                                
                            },function(){
                                $rootScope.$storage.BatchId = $rootScope.$storage.BatchId + 1;
                            console.log("$rootScope.$storage.BatchId"+$rootScope.$storage.BatchId);
                                
                                
                                
                                
                                
                                
                            }
                            );
                            
                          
                       
                             $scope.loadingGifDisplay.display = "none";
                          
                            
                            
                             ModalService.showModal({
            templateUrl: 'templates/dialogs/modalDialog.html',
            controller: "ModalController"
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                
            });
        }); 
                            
                       if($scope.verified==1)
              $scope.readVerifiedFromTable();
              if($scope.pending==1){
                 
                 $scope.readPendingFromTable();}
              if($scope.failed==1)
                  $scope.readFailedFromTable();     
                            
                            
                            
                            
                            
                            
                            
                      }); 
                     

                  });
                               
                               
                               
                               
                          });
                              
                              
                              
                              
                              
                              
                              
                              
                          }).error(function(data,status,headers,config){
                              
                              
                          });
                      
                                                   
                          
                          
                          
            
                          
                    
                          
                      }
                                        
               } );
              
         
              
              
          }
            else{
                //user opened the app in offline and switched the net connection on later
                console.log("going to login");
                
                
     ModalService.showModal({
            templateUrl: 'templates/dialogs/goToLoginDialog.html',
            controller: "ModalController"
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                 $state.go('login');
            });
        }); 
               
            }  
              
              
          }
else{
    
    //prompt to go to login page 
    
   
    
    
     ModalService.showModal({
            templateUrl: 'templates/dialogs/networkStatusDialog.html',
            controller: "ModalController"
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                
            });
        }); 
    
    
}

          }



          var insertIntoTable = function (dataToInsert) {
              console.log("datatoisnert" + dataToInsert);

              console.log("datatoisnert" + dataToInsert.Result.length);
              $rootScope.db.transaction(function (tx) {

                      for (var i = 0; i < dataToInsert.Result.length; i++) {
console.log("product name"+dataToInsert.Result[i].ProductName);
                          console.log("dataToInsert.Result[i].RetailerID" + dataToInsert.Result[i].RetailerID);
                          tx.executeSql('INSERT INTO ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail) VALUES ("' + dataToInsert.Result[i].RetailerID + '","' + dataToInsert.Result[i].AgentID + '","' + dataToInsert.Result[i].BarcodeValue + '","' + dataToInsert.Result[i].ProductID + '","' + dataToInsert.Result[i].isValid + '","' + dataToInsert.Result[i].isAgentNotified + '","' + dataToInsert.Result[i].ValidationResponse + '","' + dataToInsert.Result[i].ScanDate + '","' + dataToInsert.Result[i].ScanLocationLat + '","' + dataToInsert.Result[i].ScanLocationLong + '","' + dataToInsert.Result[i].BatchID + '","' + dataToInsert.Result[i].isProcessed + '","' + dataToInsert.Result[i].CountryCode + '","' + dataToInsert.Result[i].ProductName + '","' + dataToInsert.Result[i].ProductImage + '",1,"'+$rootScope.$storage.AgentEmail+'")');

                      }

                  }, function (error) {

                      console.log('Transaction ERROR: ' + error.message);
                  },
                  function () {
                      console.log('populated database OK');
  
                      if($scope.verified==1){
                   console.log("reading verifed");
              $scope.readVerifiedFromTable();
               }
                       if($scope.pending==1){
                           $scope.readPendingFromTable();
                       }
                       if($scope.failed==1)
                  $scope.readFailedFromTable();

                  });

          }
          $scope.onPullError = function (data, status, headers, config) {

              console.log("caught error");
          }

    $rootScope.getAllDataFromServer = function () {
                  $timeout(function () {
                      $scope.loadingGifDisplay = {
                          "display": "block"
                      };
                      
                      $rootScope.db.transaction(function (tx) {
                          tx.executeSql('DELETE  FROM ScanResults',[], function (tx, rs) {
                    // console.log("deleted");
                  }, function (tx, error) {
                     // console.log('SELECT error: ' + error.message);
                  });
                          /*tx.executeSql('CREATE TABLE IF NOT EXISTS ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail)');*/

                      }, function (error) {
                         // console.log('Transaction ERROR: ' + error.message);
                      }, function () {
                         // console.log('created table OK');
                         // console.log('https://proscanmobiletest.azurewebsites.net/api/ScanResults?retailerId=' + $rootScope.retailerId);
                          $http.get($rootScope.baseURL+'api/ScanResults?retailerId=' + $rootScope.retailerId).success($scope.onPullSuccess).error($scope.onPullError);
                      });


                  }, 3000);

              }

          $scope.onStartDateChanged = function () {

              console.log("start" + $scope.startDate);
              
              if($scope.verified==1)
              $scope.readVerifiedFromTable();
              if($scope.pending==1){
                  console.log("calling read pending");
                 $scope.readPendingFromTable();}
              if($scope.failed==1)
                  $scope.readFailedFromTable();

          }
          $scope.onEndDateChanged = function () {

              console.log("end" + $scope.endDate);
              if($scope.verified==1)
              $scope.readVerifiedFromTable();
              if($scope.pending==1){
                  console.log("Calling readpending");
                 $scope.readPendingFromTable();}
              if($scope.failed==1)
                  $scope.readFailedFromTable();
          }
          //$scope.verified=1;
         // var networkAvailability = navigator.onLine;
          if ($rootScope.checkConnection() == true) {
              var recordCount=0;
              console.log("network available");
               $rootScope.db.transaction(function (tx) {

                  tx.executeSql('SELECT count(*) AS mycount FROM ScanResults', [], function (tx, rs) {
                      console.log('Record count : ' + rs.rows.item(0).mycount);
                     recordCount=rs.rows.item(0).mycount;
                  }, function (tx, error) {
                      console.log('SELECT error: ' + error.message);
                  });
               }, function (error) {

                          console.log('Transaction ERROR: ' + error.message);
                      },
                      function () {
                          console.log('read database');
                          // readFromTable();
                  /* if(recordCount==0){
                       console.log("record count=0");
                       $rootScope.getAllDataFromServer();
                   } 
                   else{
                       $scope.loadingGifDisplay = {
                  "display": "none"
              };
                       console.log("set display to none");
                       
                       if($scope.verified==1){
              $scope.readVerifiedFromTable();}
                       if($scope.pending==1){
                           $scope.readPendingFromTable();
                       }
                       if($scope.failed==1)
                  $scope.readFailedFromTable();
                   }*/
                   
                  // if(recordCount>0){
                        $scope.loadingGifDisplay = {
                  "display": "none"
              };
                       console.log("set display to none");
                       
                       if($scope.verified==1){
              $scope.readVerifiedFromTable();}
                       if($scope.pending==1){
                           $scope.readPendingFromTable();
                       }
                       if($scope.failed==1)
                  $scope.readFailedFromTable();
                       
                  // }
                                        
               } );
              
          } else {
              $scope.loadingGifDisplay = {
                  "display": "none"
              };
               if($scope.verified==1){
                   console.log("reading verifed");
              $scope.readVerifiedFromTable();
               }
                       if($scope.pending==1){
                           $scope.readPendingFromTable();
                       }
                       if($scope.failed==1)
                  $scope.readFailedFromTable();
                   }
          

          $scope.onBackClicked = function () {

              $state.go('landing');
          }
          
          
 //$scope.readVerifiedFromTable();
      });



      app.controller("productValidationController", function ($scope, $rootScope, $state, $http,$localStorage) {
          $rootScope.currentScreen = "productValidationScreen";

          console.log("in validatn ctrl");

          var getTime=function(){
          
          var today = new Date();
                                  var dd = today.getUTCDate();
                                  var mm = today.getUTCMonth() + 1; //January is 0!

                                  var yyyy = today.getUTCFullYear();
                                  if (dd < 10) {
                                      dd = '0' + dd
                                  }
                                  if (mm < 10) {
                                     mm = '0' + mm
                                  }
                                  var dateInFormat = yyyy + '-' + mm + '-' + dd;
                                  var minute = today.getUTCMinutes();
                                  if (minute < 10) {
                                      minute = '0' + minute;
                                  }
                                  var hour = today.getUTCHours();
                                  if (hour < 10) {
                                      hour = '0' + hour;
                                  }
                                  $scope.currentTime = hour + ":" + minute;
                                  $scope.currentDate = dateInFormat;
                                  
                                  $scope.currentDateTime = $scope.currentDate + "T" + $scope.currentTime;
          
          }
          
          getTime();
          
                 
              var checkServerData=function(data){
                  console.log("data is "+data);
                    
        
                  
              }
          var onLocSuccess = function (position) {

              console.log('Latitude: ' + position.coords.latitude + '\n' +
                  'Longitude: ' + position.coords.longitude + '\n');
              $rootScope.geoLocLatValues = position.coords.latitude;
              $rootScope.geoLocLongtValues = position.coords.longitude;
              console.log("position.coords.latitude" + position.coords.latitude);

              console.log("position.coords.longitude" + position.coords.longitude);

              
              $rootScope.$storage.scanLat=$rootScope.geoLocLatValues;
              $rootScope.$storage.scanLong=$rootScope.geoLocLongtValues;
              $scope.loadingGifDisplay = {
                  "display": "block"
              };
              // $rootScope.scannedValue=parseInt($rootScope.scannedValue);
              
              var validationPostData={
                  "Barcode": $rootScope.scannedValue, 
                  "Latitude": $rootScope.geoLocLatValues,  
                  "Longitude": $rootScope.geoLocLongtValues, 
                  "RetailerId": $rootScope.retailerId   
                  
              };
              
              console.log("validationPostData"+JSON.stringify(validationPostData));
              $http({    
                          url:   'https://proscanmobile.azurewebsites.net/api/Products',
                              method:   'POST',
                              contentType:   "application/json",
                              data: validationPostData,
                      }).success(               
                  $scope.getProductFromBarcode).error($scope.errorInProductFromBarcode);
                  
                  
              
              
              
              
             /* $scope.url = "http://proscanmobile.azurewebsites.net/api/Products/" + $rootScope.scannedValue + "?" + "latitude=" + $rootScope.geoLocLatValues + "&" + "longitude=" + $rootScope.geoLocLongtValues + "&" + "retailerId=" + $rootScope.retailerId;


              console.log("url" + $scope.url);
              $http.get($scope.url).success($scope.getProductFromBarcode).error($scope.errorInProductFromBarcode);*/

          };

          function onLocError(error) {
              console.log('code: ' + error.code + '\n' +
                  'message: ' + error.message + '\n');
          }

          navigator.geolocation.getCurrentPosition(onLocSuccess, onLocError);

          $scope.onBackClicked = function () {
              $state.go('scanOptions');
          }

          $scope.getProductFromBarcode = function (data, status, headers, config) {
              console.log("got success response for barcode");
              $scope.loadingGifDisplay.display = "none";
console.log("data is "+data);
              $scope.productScannedDetails = [];

              
              
                    if (data == "Product Not Found") {
                  
                   $scope.productScannedDetails.push({
                      "prodImage": "assets/images/slices/invalidProd.jpg",
                      "prodName": "This product is not identified with Syngenta"

                  });
                  console.log("product not found condition");
                   $rootScope.db.transaction(function (tx) {
                  
                    tx.executeSql('INSERT INTO ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail) VALUES ("' + $rootScope.retailerId + '","' + $rootScope.agentId + '","' + $rootScope.scannedValue + '","' + '' + '","' + 'false' + '","' + 'false' + '","' + 'Invalid' + '","' + $scope.currentDateTime + '","' + $rootScope.geoLocLatValues + '","' + $rootScope.geoLocLongtValues + '",' + null + ',"' + 'true' + '","' + '' + '","invalid product","assets/images/slices/invalidProd.jpg",1,"'+$rootScope.$storage.AgentEmail+'")');
 $scope.$apply();
                   },function(error){
                        console.log('Transaction ERROR: ' + error.message);
                   },
                     function(){
                        console.log('inserted values OK for product not found');
                   });
                  
                 
//$scope.$apply();
                  console.log("$scope.productScannedDetails.prodImage"+$scope.productScannedDetails.prodImage);
                
                  
                  
              } else {
                  
                   $scope.productScannedDetails.push({
                      "prodImage": data.ProfileImage,
                      "prodName": data.description
                  });
                  console.log("data.ProfileImage" + data.ProfileImage + "\ndata.description" + data.description);
                  
                  
                  
                   $rootScope.db.transaction(function (tx) {
                  
                    tx.executeSql('INSERT INTO ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail) VALUES ("' + $rootScope.retailerId + '","' + $rootScope.agentId + '","' + $rootScope.scannedValue + '","' + '' + '","' + 'true' + '","' + 'false' + '","' + 'Valid Code' + '","' + $scope.currentDateTime + '","' + $rootScope.geoLocLatValues + '","' + $rootScope.geoLocLongtValues + '",' + null + ',"' + 'true' + '","' + '' + '","'+data.description+'","'+data.ProfileImage+'",1,"'+$rootScope.$storage.AgentEmail+'")');
 $scope.$apply();
                   },function(error){
                        console.log('Transaction ERROR: ' + error.message);
                   },
                     function(){
                        console.log('inserted values OK for product found');
                   });
                  
                 

                 // $scope.$apply();
              }
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
             // checkServerData(data);
             /* $rootScope.db.transaction(function (tx) {
                          tx.executeSql('CREATE TABLE IF NOT EXISTS ScanResults (RetailerID, AgentID,BarcodeValue,ProductID,isValid,isAgentNotified,ValidationResponse,ScanDate,ScanLocationLat,ScanLocationLong,BatchID,isProcessed,CountryCode,ProductName,ProductImage,syncStatus,agentEmail)');


                        



                      }, function (error) {

                          console.log('Transaction ERROR: ' + error.message);
                      },
                      function () {
                          console.log('created database OK');
                          // readFromTable();
                       

                      });*/
              
              
              
              
       
              
              
              
              
              
            



          }
          $scope.errorInProductFromBarcode = function (data, status, headers, config) {
              console.log("got error response for barcode");
              console.log("error hitting service");

          }

          $scope.onTickClicked = function () {
              $state.go('scanOptions');
          }


      });

app.controller('ModalController', function($scope, close) {
  
 $scope.close = function(result) {
               close(result, 500); // close, but give 500ms for bootstrap to animate
};

      });


   app.filter("myfilter", function($filter) {
          console.log("entered filter");
      return function(items, from, to) {

            return $filter('filter')(items, "name", function(v){
               // console.log("sas");
              var date  = moment(v);
             // console.log(date);
              return date >= moment(from) && date <= moment(to);
            });
      };
    });
