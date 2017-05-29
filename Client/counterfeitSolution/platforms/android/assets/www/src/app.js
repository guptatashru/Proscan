



 var app = angular.module('proScanApp', ["ui.router","ngCordova","ngStorage","angularModalService"]);
   app.value('multipleScanItems',[{
       productName:"",
                                    "barcodeNumber":"",
                                    "captureTime":""
   }]);
 app.config (['$urlRouterProvider','$stateProvider',function ($urlRouterProvider, $stateProvider){
	
 
     
	 $urlRouterProvider.otherwise('/');  
 $stateProvider

    .state('login', {                                                     //setting up different states in module config
        url: '/',
        templateUrl: 'templates/partials/loginPage.html',
        controller: 'loginController'
    })
  .state('landing', {                                                     //setting up different states in module config
        url: '/landing',
        templateUrl: 'templates/partials/landingPage.html',
        controller: 'landingController'
    })
     .state('register',{
     url:'/register',
     templateUrl:'templates/partials/registrationPage.html',
     controller:'registrationController'
 })
 
.state('productValidationResult',{
     url:'/productValidationResult',
     templateUrl:'templates/partials/productValidationResult.html',
     controller:'productValidationController'
})
 .state('scanOptions',{
     url:'/scanOptions',
     templateUrl:'templates/partials/intermediatePage.html',
     controller:'scanOptionsController'
})

 .state('productCurrentScanList',{
     url:'/productCurrentScanList',
     templateUrl:'templates/partials/productScanList.html',
     controller:'productScanCurrentListController'
})
 
  .state('productScanHistory',{
     url:'/productScanHistory',
     templateUrl:'templates/partials/productScanHistory.html'
})
.state('productShareDetails',{
     url:'/productShareDetails',
     templateUrl:'templates/partials/productShareDetails.html',
     controller:'productShareDetailsController'
});
	
 
	
 }]);

app.run(function ($rootScope) {

  /*$rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
  
        
    if(toState.name === "landing"){
       return;
    }

    if($rootScope.checkConnection()==false) {

        // this should not be commented
        //event.preventDefault();
        // because here we must stop current flow.. .
        console.log("going to landing");
        event.preventDefault();
        $state.transitionTo("landing", null, {notify:false});
        $state.go('landing');
    }
  });*/

});
  