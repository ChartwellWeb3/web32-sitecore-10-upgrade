﻿var income =0;
	var gbTemp =0;
	var pensionTemp =0; 
	var oasTemp =0;
	var gisTemp =0;
	var benefitsTemp =0;
	var retirementTemp =0;
	var RRSPTemp =0;
	var RRIFTemp =0;
	var LRIFTemp =0;
	var investmentTemp =0;
	var companyPensionTemp =0;
	var employmentTemp =0;
	var otherTemp =0;
	var rentalTemp =0;
	var otherIncomeTemp = 0; 
	
	var cashProceeds = 0;
	var homeIncome = 0; 
	var totalIncome = 0;
	 
	var homeValue = 0; 
	var mortgage = 0; 
	var legal = 0; 
	var commission = 0; 
	var movingCosts = 0; 
	var defaultRate = 0; 
	var ownRate = 0; 
	var rateOfReturn = 1; 
	
	var housingExpense = 0;
	var housingMaintenance = 0;  
	var livingExpense = 0; 
	var transportationCost = 0; 
	 
	var mortgageCost = 0; 
	var rent = 0;
	var condo = 0; 
	var propertyTaxes = 0; 
	var utilitiesGas = 0; 
	var utilitiesElectricity = 0; 
	var otherUtilities = 0; 
	
	var premium = 0; 
	var tenantInsurance = 10; 
	var insuranceExpense = 0; 
	
	var lawnCare = 0; 
	var landscaping = 0; 
	var snowRemoval = 0; 
	var security = 0; 
	var windowClean = 0; 
	var garbageCollection = 0; 
	var roof = 0;
	var furnace = 0; 
	var airConditioning = 0; 
	var appliances = 0; 
	var emergencyRepairs = 0;
	var otherMaintenance = 0;  
	var otherCosts = 0; 
	  
	var foodExpense = 0; 
	var mealChoice = "default"; 
	var otherMealCosts = 0; 
	var foodSaving = 0; 
	
	var entertainment = 0; 
	var livingExpense = 0; 
	
	var loan = 0;
	var gas = 0;
	var carInsurance = 0; 
	var carMaintenance =0;
	var license = 0; 
	var parking = 0; 
	var carWash = 0; 
	var repairs = 0; 
	var otherTransportation = 0; 
	var transportationCost = 0; 
	
	var monthlyIncome = 0; 
	var additionalIncome = 0; 
	var cashFromHome = 0; 
	var monthlyExpense = 0; 
	
	var numErrors = 0; 
	var errors = new Array(); 
	
	var textFields = ["pension","oas","gis","benefitsOther","RRSP","RRIF","LRIF","investment","companyPension","employment","rental","otherIncome","homeValue","mortgage","legal","commission","movingCosts","ownRate","mortgageCost","rent","condo","propertyTaxes","utilitiesGas","utilitiesGas","utilitiesElectricity","otherUtilities","premium","lawnCare","landscaping","snowRemoval","security","windowClean","garbageCollection","roof","furnace","airConditioning","appliances","emergencyRepairs","otherMaintenance","otherCosts","foodExpense","entertainment","loan","gas","carInsurance","carMaintenance","license","parking","carWash","repairs", "otherTransportation"];
	
	window.onload = init;
	
	 $(document).keydown(function(e) {
		var element = e.target.nodeName.toLowerCase();
		if (element != 'input') {
    		if (e.keyCode === 8) {
       			return false;
    		}
		}
	});
	
	 $(document).keypress(function(e) {
		var element = e.target.nodeName.toLowerCase();
		if (element != 'input') {
    		if (e.keyCode === 8) {
       			return false;
    		}
		}
	});
	
	$(document).ready(function() { // FF needs keypress, IE needs keydown
            $('select').keypress(function(event) 
               { return cancelBackspace(event) });
            $('select').keydown(function(event) 
               { return cancelBackspace(event) });
        }); 
		
	function cancelBackspace(event) {
            if (event.keyCode == 8) {
                return false;
            }
        }
	
	
	function init () {
		
		for (var i=0; i<textFields.length; i++){
			document.getElementById(textFields[i]).value="";
		}
		 document.getElementById("defaultSell").selected = "true";
		 document.getElementById("defaultDisplay").selected = "true";
		 document.getElementById("defaultChoiceDisplay").selected = "true";
		 document.getElementById("defaultVechicleChoiceDisplay").selected = "true";
	
	}
	
	function calculateIncome(){
		income = gbTemp + pensionTemp + oasTemp + gisTemp + benefitsTemp + retirementTemp + RRSPTemp + RRIFTemp + LRIFTemp + investmentTemp + companyPensionTemp + employmentTemp + otherTemp + rentalTemp + otherIncomeTemp;
		document.getElementById("total").innerHTML = "<p> $ "+numberWithCommas(income.toFixed(0))+"</p>";
		totalIncome = homeIncome + income; 
		document.getElementById("totalIncome").innerHTML = "<p> $"+numberWithCommas(totalIncome.toFixed(0))+"</p>"; 
		monthlyIncome = income;
		document.getElementById("monthlyIncome").innerHTML = "<p> $ " + numberWithCommas(monthlyIncome.toFixed(0))+ "</p>"; 
		////console.log(document.getElementById("nextIncome")); 
	}
	
	function calculateCashProceed(){
		cashProceeds = homeValue - mortgage - legal - commission - movingCosts;
		if (cashProceeds < 0){
			document.getElementById("cashProceedsError").style.display = "block";
			document.getElementById("cashProceedsError1").style.display = "block";
			document.getElementById("cashProceedsError1").innerHTML = "<p>Your cash proceeds reflect a negative value. Please verify and adjust the numbers you have entered for your estimated home value, mortgage, legal fees, realtor commission fees, or moving costs in the Home Equity section.</p>";
		}
		else {
			document.getElementById("cashProceedsError").style.display = "none";
			document.getElementById("cashProceedsError1").style.display = "none"; 
		}
		document.getElementById("cashProceeds").innerHTML = "<p> $ " + numberWithCommas(cashProceeds.toFixed(0))+ "</p>";
		cashFromHome = cashProceeds; 
		document.getElementById("cashFromHome").innerHTML = "<p> $ " + numberWithCommas(cashFromHome.toFixed(0))+ "</p>";  
	}
	
	function calculateHomeEquity(){
		
		homeIncome = (cashProceeds * rateOfReturn)/12; 
		document.getElementById("homeIncome").innerHTML = "<p> $ " + numberWithCommas(homeIncome.toFixed(0))+ "</p>";
		additionalIncome = homeIncome; 
		document.getElementById("additionalIncome").innerHTML = "<p> $ " + numberWithCommas(additionalIncome.toFixed(0))+"</p>";
		totalIncome = additionalIncome + income;
		//console.log (" Here again again" + totalIncome);
		document.getElementById("totalIncome").innerHTML = "<p> $ " + numberWithCommas(totalIncome.toFixed(0))+"</p>";  
	}
	
	function calculateAdditionalIncome (){
		
		if (homeIncome == 0){
			calculateCashProceed();
		}
		else {
			calculateCashProceed();
			calculateHomeEquity();	
		}
	}
	
	function sellHome() {
		
		var sellHome = document.getElementById("home_sell").value;
		
		if (sellHome =="yes"){
			document.getElementById("saleIncome").style.display = "block";
		}
		
		else if (sellHome =="no") {
			homeValue = 0; 
			mortgage = 0; 
			legal = 0; 
			commission = 0; 
			movingCosts = 0; 
			defaultRate = 0;
			ownRate = 0; 
			rateOfReturn = 0;
			calculateCashProceed();
			calculateHomeEquity(); 
			document.getElementById("homeValue").value = ""; 
			document.getElementById("mortgage").value = ""; 
			document.getElementById("legal").value = ""; 
			document.getElementById("commission").value = ""; 
			document.getElementById("movingCosts").value = ""; 
			document.getElementById("ownRate").value = ""; 
			document.getElementById("defaultDisplay").selected = "true";
			document.getElementById("saleIncome").style.display = "none";
			document.getElementById("rateOfReturn").style.display ="none"; 
			document.getElementById("own").style.display ="none";
			document.getElementById("homeError1").style.display = "none";
			document.getElementById("homeError2").style.display = "none";
			document.getElementById("mortgageError").style.display = "none";
			document.getElementById("mortgageError1").style.display = "none";
			document.getElementById("legalError").style.display = "none";
			document.getElementById("legalError1").style.display = "none";
			document.getElementById("commissionError").style.display = "none";
			document.getElementById("commissionError1").style.display = "none";
			document.getElementById("movingCostsError").style.display = "none";
			document.getElementById("movingCostsError1").style.display = "none"; 
		}
		
	}
	
	function noBackspace(){
		
		if (event.keyCode ==8){
			
			return false; 
		}	
	}
	
	
	function calculateInsuranceExpenses () {
		insuranceExpense = premium - tenantInsurance; 
		document.getElementById("insuranceExpense").innerHTML = "<p> $ " +numberWithCommas(insuranceExpense.toFixed(0))+"</p>";
		housingExpense = mortgageCost + rent + condo + propertyTaxes + utilitiesGas + utilitiesElectricity + otherUtilities + premium; 
		document.getElementById("housingExpense").innerHTML = "<p> $ " +numberWithCommas((housingExpense- tenantInsurance).toFixed(0))+"</p>"; 
		monthlyExpense = housingExpense + housingMaintenance + livingExpense + transportationCost - tenantInsurance; 
		document.getElementById("monthlyExpense").innerHTML = "<p> $ " +numberWithCommas(monthlyExpense.toFixed(0))+"</p>"; 
	}
	
	function calculateHousingExpenses(){
		insuranceExpense = premium - tenantInsurance; 
		housingExpense = mortgageCost + rent + condo + propertyTaxes + utilitiesGas + utilitiesElectricity + otherUtilities + premium; 
		document.getElementById("housingExpense").innerHTML = "<p> $ " +numberWithCommas((housingExpense-tenantInsurance).toFixed(0))+"</p>"; 
		monthlyExpense = housingExpense + housingMaintenance + livingExpense + transportationCost - tenantInsurance; 
		document.getElementById("monthlyExpense").innerHTML = "<p> $ " +numberWithCommas(monthlyExpense.toFixed(0))+"</p>"; 
		
	}
	
	function calculateHousingMaintenance(){
		housingMaintenance = lawnCare + landscaping + snowRemoval + security + windowClean + garbageCollection + roof + furnace + airConditioning
		+ appliances + emergencyRepairs + otherMaintenance + otherCosts;
		document.getElementById("housingMaintenance").innerHTML = "<p> $ " +numberWithCommas((housingMaintenance).toFixed(0))+"</p>"; 
		monthlyExpense = housingExpense + housingMaintenance + livingExpense + transportationCost - tenantInsurance; 
		document.getElementById("monthlyExpense").innerHTML = "<p> $ " +numberWithCommas(monthlyExpense.toFixed(0))+"</p>"; 
		
	}
	
	function calculateFoodSaving() {
		otherMealCosts = foodExpense*mealChoice; 
		document.getElementById("otherMealCosts").innerHTML = "<p> $ " +numberWithCommas(otherMealCosts.toFixed(0))+"</p>"; 
		foodSaving = foodExpense - otherMealCosts; 
		document.getElementById("foodSaving").innerHTML = "<p> $ " +numberWithCommas(foodSaving.toFixed(0))+ "</p>";
		livingExpense = foodSaving + entertainment; 
		document.getElementById("livingExpense").innerHTML = "<p> $ " +numberWithCommas(livingExpense.toFixed(0))+"</p>";
		monthlyExpense = housingExpense + housingMaintenance + livingExpense + transportationCost - tenantInsurance; 
		document.getElementById("monthlyExpense").innerHTML = "<p> $ " +numberWithCommas(monthlyExpense.toFixed(0))+"</p>"; 
		 
	}
	
	function calculateLivingExpense() {
		livingExpense = foodSaving + entertainment; 
		document.getElementById("livingExpense").innerHTML = "<p> $ " +numberWithCommas(livingExpense.toFixed(0))+"</p>";
		monthlyExpense = housingExpense + livingExpense + housingMaintenance + transportationCost - tenantInsurance; 
		document.getElementById("monthlyExpense").innerHTML = "<p> $ " +numberWithCommas(monthlyExpense.toFixed(0))+"</p>";
	}
	
	function resetMealPlan(){
		
		mealChoice = "default";
		otherMealCosts = 0; 
		foodSaving = 0; 
		document.getElementById("otherMealCosts").innerHTML = "";
		document.getElementById("foodSaving").innerHTML = "";
		//document.getElementById("livingExpense").innerHTML =  "<p> $ " +numberWithCommas(livingExpense.toFixed(0))+"</p>";
		
	}
	
	function calculateTransportationCost(){
		transportationCost = loan + gas + carInsurance + carMaintenance + license + parking + carWash + repairs + otherTransportation; 
		document.getElementById("transportationCost").innerHTML = "<p> $ " +numberWithCommas(transportationCost.toFixed(0)) +"</p>";
		monthlyExpense = housingExpense + livingExpense + transportationCost + housingMaintenance - tenantInsurance; 
		document.getElementById("monthlyExpense").innerHTML = "<p> $ " +numberWithCommas(monthlyExpense.toFixed(0))+"</p>";  
	}	
	
	function numberWithCommas(x) {
    	var y;
		var result;  
		y = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		if (y.charAt(0)=="-"){
			result = "(" + y.substring(1,y.length)+")";	
		}
		else {
			result = y;
		}
		return result;
    }
	
	function toNumber(y){
		return y.toString().replace(/[, ]+/g, "");
	}
	
	function trimInput (x) {
		
		var numX = x.length;
		var result = ""; 
		for (var i = 0; i<numX; i++){
			var temp = x.charAt(i);
			if (temp==" " || temp==",") {
				
			}
			else {
				result = result + temp;
			}
		}
		//result.trim();
		//console.log(result);
			return result;
		
	}
	
	function trimInputLess (x) {
		
		var numX = x.length;
		var result = ""; 
		
		for (var i = 0; i<numX; i++){
			var temp = x.charAt(i);
			if (temp==" " || temp==",") {
				
			}
			else {
				result = result + temp;
			}
		}
			
			var subX = ""; 
		
		if (result.charAt(0)=="("){
			if (result.charAt(result.length-1)==")")
			{
				subX = result.substring (1, result.length-1);
			}
			else {
				subX = result.substring (1, result.length);
				
			}
		}
		else {
			if (result.charAt(result.length-1)==")")
			
			{
				subX = result.substring (0, result.length-1);
			
			}
			else {
				subX = result.substring (0, result.length);
			}
		}
		
			return subX;	
	}
	
	function displayError () {
		if (errors.length>0){
			document.getElementById("errorMsg").style.display = "block";
			document.getElementById("errorOverall").innerHTML = errors;
		}
		else {
			document.getElementById("errorMsg").style.display = "none";
			document.getElementById("errorOverall").innerHTML = "";
		
		}
	}
	

	function addBenefits() {
		
		var myTemp = trimInput(document.getElementById("gb").value); 
		
		if (myTemp =="")
		{
			gbTemp = 0; 
			document.getElementById("benefitsError").style.display = "none";
			document.getElementById("gb").value = "0";
			
		}
		else {
			
			gbTemp = parseFloat(myTemp);
			
			if(isNaN(gbTemp)){
				document.getElementById("benefitsError").style.display = "block";
				document.getElementById("benefitsError").innerHTML = "<p>Please enter a numeric value.</p>"
				gbTemp = 0; 
			}
			else if (gbTemp < 0){
				document.getElementById("benefitsError").style.display = "block";
				document.getElementById("benefitsError").innerHTML = "<p>Please enter a positive value.</p>"
				gbTemp = 0; 
			}
			else {
				document.getElementById("benefitsError").style.display = "none";
				document.getElementById("gb").value = numberWithCommas(gbTemp);
			}
		}
		calculateIncome();
		
	}
	
	//var isError = false;
	
	function addPension() {
		
		var myTemp = trimInput(document.getElementById("pension").value);
		
		//console.log ("Here "+ isNaN(myTemp)); 
		
		var errorNum;  
		
		if (myTemp =="")
		{
			pensionTemp = 0; 
			document.getElementById("pensionError").style.display = "none";
			document.getElementById("pension").value = "0";
			document.getElementById("pensionError1").style.display = "none";
			//document.getElementById("errorMsg").style.display = "none";
			
		}
		
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("pensionError").style.display = "block";
				document.getElementById("pensionError").innerHTML = "<p>Please enter a numeric value.</p>";
				pensionTemp = 0; 
				
				//document.getElementById("errorMsg").style.display = "block";
				document.getElementById("pensionError1").style.display = "block";
				document.getElementById("pensionError1").innerHTML = "<p>Please enter a numeric value for CPP/QPP in the Monthly Income section.</p>";
			}
			
			else {
				
				pensionTemp = parseFloat(myTemp);
				
				if (pensionTemp<0){
					document.getElementById("pensionError").style.display = "block";
					document.getElementById("pensionError").innerHTML = "<p>Please enter a positive number.</p>";
					pensionTemp = 0;
					//document.getElementById("errorMsg").style.display = "block";
					document.getElementById("pensionError1").style.display = "block";
					document.getElementById("pensionError1").innerHTML = "<p>Please enter a positive number for CPP/QPP in the Monthly Income section.</p>";
				}
				
				else if (pensionTemp>10000000){
					document.getElementById("pensionError").style.display = "block";
					document.getElementById("pensionError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					pensionTemp = 0;
					//document.getElementById("errorMsg").style.display = "block";
					document.getElementById("pensionError1").style.display = "block";
					document.getElementById("pensionError1").innerHTML = "<p>The value for CPP/QPP in the Monthly Income section must be less than 10,000,001.</p>";
				}
				
				else {
					document.getElementById("pensionError").style.display = "none";
					document.getElementById("pension").value = numberWithCommas(pensionTemp.toFixed(0));
					document.getElementById("pensionError1").style.display = "none";
					//document.getElementById("errorMsg").style.display = "none";
				}
			}
		}
		
		calculateIncome();
	}
	
	function addOas() {
		var myTemp = trimInput(document.getElementById("oas").value);
		
		if (myTemp==""){
			
			oasTemp = 0; 
			//console.log (parseFloat(myTemp));
			document.getElementById("oasError").style.display = "none";
			document.getElementById("oas").value = "0";
			document.getElementById("oasError1").style.display = "none";
			//document.getElementById("errorMsg").style.display = "none";
			
		}
		
		else {
		
			
			if(isNaN(myTemp)){
				document.getElementById("oasError").style.display = "block";
				document.getElementById("oasError").innerHTML = "<p>Please enter a numeric value.</p>";
				oasTemp = 0; 
				//document.getElementById("errorMsg").style.display = "block";
				document.getElementById("oasError1").style.display = "block";
				document.getElementById("oasError1").innerHTML = "<p>Please enter a numeric value for Old Age Security in the Monthly Income section.</p>";
				
			}
			
			else {
				
				oasTemp = parseFloat(myTemp);
				if (oasTemp<0){
					document.getElementById("oasError").style.display = "block";
					document.getElementById("oasError").innerHTML = "<p>Please enter a positive number.</p>";
					oasTemp = 0;
					//document.getElementById("errorMsg").style.display = "block";
					document.getElementById("oasError1").style.display = "block";
					document.getElementById("oasError1").innerHTML = "<p>Please enter a positive number for Old Age Security in the Monthly Income section.</p>";
				 
				}
				else if (oasTemp>10000000){
					document.getElementById("oasError").style.display = "block";
					document.getElementById("oasError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					oasTemp = 0;
					//document.getElementById("errorMsg").style.display = "block";
					document.getElementById("oasError1").style.display = "block";
					document.getElementById("oasError1").innerHTML = "<p>The value for Old Age Security in the Monthly Income section must be less than 10,000,001.</p>";
				 
				}
				else {
					document.getElementById("oasError").style.display = "none";
					document.getElementById("oas").value = numberWithCommas(oasTemp.toFixed(0));
					//console.log(oasTemp);
					document.getElementById("oasError1").style.display = "none";
					//document.getElementById("errorMsg").style.display = "none";
					
					
				}
			}
		}
		
		calculateIncome();
	}
	
	function addGis() {
		
		var myTemp = trimInput(document.getElementById("gis").value);
		
		if (myTemp ==""){
			
			gisTemp = 0;
			document.getElementById("gisError").style.display = "none";
			document.getElementById("gis").value = "0"; 
			document.getElementById("gisError1").style.display = "none";
			
		}
		
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("gisError").style.display = "block";
				document.getElementById("gisError").innerHTML = "<p>Please enter a numeric value.</p>";
				gisTemp = 0; 
				
				document.getElementById("gisError1").style.display = "block";
				document.getElementById("gisError1").innerHTML = "<p>Please enter a numeric value for Guaranteed Income Supplement in the Monthly Income section.</p>";
			}
			
			else {
				
				gisTemp = parseFloat(myTemp);
				
				if (gisTemp <0){
					document.getElementById("gisError").style.display = "block";
					document.getElementById("gisError").innerHTML = "<p>Please enter a positive number.</p>";
					gisTemp = 0;
					
					document.getElementById("gisError1").style.display = "block";
					document.getElementById("gisError1").innerHTML = "<p>Please enter a positive number for Guaranteed Income Supplement in the Monthly Income section.</p>"; 
				}
				
				else if (gisTemp > 10000000){
					document.getElementById("gisError").style.display = "block";
					document.getElementById("gisError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					gisTemp = 0;
					
					document.getElementById("gisError1").style.display = "block";
					document.getElementById("gisError1").innerHTML = "<p>The value for Guaranteed Income Supplement in the Monthly Income section must be less than 10,000,001.</p>"; 
				}
				
				else {
					document.getElementById("gisError").style.display = "none";
					document.getElementById("gis").value = numberWithCommas(gisTemp.toFixed(0));
					document.getElementById("gisError1").style.display = "none";
					
				}
			}
			
			}
			calculateIncome();
		
		}
		
		function addBenefitsOther() {
			
			var myTemp = trimInput(document.getElementById("benefitsOther").value);
			
			if (myTemp ==""){
				
				benefitsTemp = 0; 
				document.getElementById("benefitsOtherError").style.display = "none";
				document.getElementById("benefitsOther").value = "0";
				document.getElementById("benefitsOtherError1").style.display = "none";
				
			}
			
			else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("benefitsOtherError").style.display = "block";
				document.getElementById("benefitsOtherError").innerHTML = "<p>Please enter a numeric value.</p>";
				benefitsTemp = 0; 
				
				document.getElementById("benefitsOtherError1").style.display = "block";
				document.getElementById("benefitsOtherError1").innerHTML = "<p>Please enter a numeric value for Post-Retirement Benefit, Disability Benefit & Other Government Benefits in the Monthly Income section.</p>";
			}
			
			else {
			
				benefitsTemp = parseFloat(myTemp);
				
				if (benefitsTemp<0){
					document.getElementById("benefitsOtherError").style.display = "block";
					document.getElementById("benefitsOtherError").innerHTML = "<p>Please enter a positive number.</p>";
					benefitsTemp = 0; 
					
					document.getElementById("benefitsOtherError1").style.display = "block";
					document.getElementById("benefitsOtherError1").innerHTML = "<p>Please enter a positive number for Post-Retirement Benefit, Disability Benefit & Other Government Benefits in the Monthly Income section.</p>";
				}
				
				else if (benefitsTemp>10000000){
					document.getElementById("benefitsOtherError").style.display = "block";
					document.getElementById("benefitsOtherError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					benefitsTemp = 0; 
					
					document.getElementById("benefitsOtherError1").style.display = "block";
					document.getElementById("benefitsOtherError1").innerHTML = "<p>The value for Post-Retirement Benefit, Disability Benefit & Other Government Benefits in the Monthly Income section must be less than 10,000,001.</p>";
				}
				
				else {
					document.getElementById("benefitsOtherError").style.display = "none";
					document.getElementById("benefitsOther").value = numberWithCommas(benefitsTemp.toFixed(0));
					document.getElementById("benefitsOtherError1").style.display = "none";
					
				}
			
			}
			
		}
		
		calculateIncome();
	}
	
	function addRetirementSaving() {
		
		myTemp = trimInput(document.getElementById("retirementSaving").value);
		
		
		
		if (myTemp ==""){
			
			retirementTemp = 0; 
			document.getElementById("retirementError").style.display = "none";
			document.getElementById("retirementSaving").value = "0";
			document.getElementById("retirementError1").style.display = "none";
			
		}
		
		else {
			
		if(isNaN(myTemp)){
			document.getElementById("retirementError").style.display = "block";
			document.getElementById("retirementError").innerHTML = "<p>Please enter a numeric value.</p>";
			retirementTemp = 0; 
			
			document.getElementById("retirementError1").style.display = "block";
			document.getElementById("retirementError1").innerHTML = "<p>Please enter a numeric value for Retirement Savings in the Monthly Income section.</p>";

		}
		
		else {
			
			retirementTemp = parseFloat(myTemp);
			
			if (retirementTemp<0){
				document.getElementById("retirementError").style.display = "block";
				document.getElementById("retirementError").innerHTML = "<p>Please enter a positive number.</p>";
				retirementTemp = 0;
				
				document.getElementById("retirementError1").style.display = "block";
				document.getElementById("retirementError1").innerHTML = "<p>Please enter a positive number for Retirement Savings in the Monthly Income section.</p>"; 
			}
			
			else if (retirementTemp>10000000){
				document.getElementById("retirementError").style.display = "block";
				document.getElementById("retirementError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
				retirementTemp = 0;
				
				document.getElementById("retirementError1").style.display = "block";
				document.getElementById("retirementError1").innerHTML = "<p>The value for Retirement Savings in the Monthly Income section must be less than 10,000,001.</p>"; 
			}
			else {
				document.getElementById("retirementError").style.display = "none";
				document.getElementById("retirementSaving").value = numberWithCommas(retirementTemp.toFixed(0));
				document.getElementById("retirementError1").style.display = "none";
				
			}
		}
		
		}
		calculateIncome();
	}
	
	function addRRSP() {
		
		var myTemp = trimInput(document.getElementById("RRSP").value); 
		
		if (myTemp =="")
		{
			RRSPTemp = 0; 
			document.getElementById("rrspError").style.display = "none";
			document.getElementById("RRSP").value = "0";
			document.getElementById("rrspError1").style.display = "none";
			
		}
		else {
		
		
		
		if(isNaN(myTemp)){
			document.getElementById("rrspError").style.display = "block";
			document.getElementById("rrspError").innerHTML = "<p>Please enter a numeric value.</p>";
			RRSPTemp = 0; 
			
			document.getElementById("rrspError1").style.display = "block";
			document.getElementById("rrspError1").innerHTML = "<p>Please enter a numeric value for RRSP in the Monthly Income section.</p>";
		}
		
		else {
			
			RRSPTemp = parseFloat(myTemp);
		
			if(RRSPTemp<0){
				document.getElementById("rrspError").style.display = "block";
				document.getElementById("rrspError").innerHTML = "<p>Please enter a positive number.</p>";
				RRSPTemp = 0;
				
				document.getElementById("rrspError1").style.display = "block";
				document.getElementById("rrspError1").innerHTML = "<p>Please enter a positive number for RRSP in the Monthly Income section.</p>"; 
			}
			
			else if(RRSPTemp>10000000){
				document.getElementById("rrspError").style.display = "block";
				document.getElementById("rrspError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
				RRSPTemp = 0;
				
				document.getElementById("rrspError1").style.display = "block";
				document.getElementById("rrspError1").innerHTML = "<p>The value for RRSP in the Monthly Income section must be less than 10,000,001.</p>"; 
			}
			else {
				document.getElementById("rrspError").style.display = "none";
				document.getElementById("RRSP").value = numberWithCommas(RRSPTemp.toFixed(0));
				document.getElementById("rrspError1").style.display = "none";
			
			}
		}
		
		}
			calculateIncome();
	}
	
	function addRRIF() {
		
		var myTemp = trimInput(document.getElementById("RRIF").value); 
		
		if (myTemp =="")
		{
			RRIFTemp = 0; 
			document.getElementById("rrifError").style.display = "none";
			document.getElementById("RRIF").value = "0";
			document.getElementById("rrifError1").style.display = "none";
			
		}
		else {
		
		
		
		if(isNaN(myTemp)){
			document.getElementById("rrifError").style.display = "block";
			document.getElementById("rrifError").innerHTML = "<p>Please enter a numeric value.</p>";
			RRIFTemp = 0; 
			
			document.getElementById("rrifError1").style.display = "block";
			document.getElementById("rrifError1").innerHTML = "<p>Please enter a numeric value for RRIF in the Monthly Income section.</p>"; 
		}
		
		else {
			
			RRIFTemp = parseFloat(myTemp);
		
			if (RRIFTemp<0){
				document.getElementById("rrifError").style.display = "block";
				document.getElementById("rrifError").innerHTML = "<p>Please enter a positive number.</p>";
				RRIFTemp = 0;
				
				document.getElementById("rrifError1").style.display = "block";
				document.getElementById("rrifError1").innerHTML = "<p>Please enter a positive number for RRIF in the Monthly Income section.</p>";  
			}
			
			else if (RRIFTemp>10000000){
				document.getElementById("rrifError").style.display = "block";
				document.getElementById("rrifError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
				RRIFTemp = 0;
				
				document.getElementById("rrifError1").style.display = "block";
				document.getElementById("rrifError1").innerHTML = "<p>The value for RRIF in the Monthly Income section must be less than 10,000,001.</p>";  
			}
			else {
				document.getElementById("rrifError").style.display = "none";
				document.getElementById("RRIF").value = numberWithCommas(RRIFTemp.toFixed(0));
				document.getElementById("rrifError1").style.display = "none";
			}
		}
		}
		calculateIncome();
	}
	
	function addLRIF() {
		
		var myTemp = trimInput(document.getElementById("LRIF").value); 
		
		if (myTemp =="")
		{
			LRIFTemp = 0; 
			document.getElementById("lrifError").style.display = "none";
			document.getElementById("LRIF").value = "0";
			document.getElementById("lrifError1").style.display = "none";
			
		}
		else {
		
			
			
			if(isNaN(myTemp)){
				document.getElementById("lrifError").style.display = "block";
				document.getElementById("lrifError").innerHTML = "<p>Please enter a numeric value.</p>";
				LRIFTemp = 0;
				
				document.getElementById("lrifError1").style.display = "block";
				document.getElementById("lrifError1").innerHTML = "<p>Please enter a numeric value for Life Income Fund (LIF) & Locked-In Retirement Income Fund (LRIF) in the Monthly Income section.</p>";   
			}
			
			else {
				
				LRIFTemp = parseFloat(myTemp);
				
				if (LRIFTemp<0){
					document.getElementById("lrifError").style.display = "block";
					document.getElementById("lrifError").innerHTML = "<p>Please enter a positive number.</p>";
					LRIFTemp = 0; 
					
					document.getElementById("lrifError1").style.display = "block";
					document.getElementById("lrifError1").innerHTML = "<p>Please enter a positive number for Life Income Fund (LIF) & Locked-In Retirement Income Fund (LRIF) in the Monthly Income section.</p>";   
				}
				
				else if (LRIFTemp>10000000){
					document.getElementById("lrifError").style.display = "block";
					document.getElementById("lrifError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					LRIFTemp = 0; 
					
					document.getElementById("lrifError1").style.display = "block";
					document.getElementById("lrifError1").innerHTML = "<p>The value for Life Income Fund (LIF) & Locked-In Retirement Income Fund (LRIF) in the Monthly Income section must be less than 10,000,001.</p>";   
				}
				else {
					document.getElementById("lrifError").style.display = "none";
					document.getElementById("LRIF").value = numberWithCommas(LRIFTemp.toFixed(0));
					document.getElementById("lrifError1").style.display = "none";
				}
			}
		
		}
		calculateIncome();
	}
	
	function addInvestment() {
		
		var myTemp = trimInput(document.getElementById("investment").value); 
		
		if (myTemp =="")
		{
			investmentTemp = 0; 
			document.getElementById("investmentError").style.display = "none";
			document.getElementById("investment").value = "0";
			document.getElementById("investmentError1").style.display = "none";
			
		}
		else {
		 
		
			
			if(isNaN(myTemp)){
				document.getElementById("investmentError").style.display = "block";
				document.getElementById("investmentError").innerHTML = "<p>Please enter a numeric value.</p>";
				investmentTemp = 0;
				
				document.getElementById("investmentError1").style.display = "block";
				document.getElementById("investmentError1").innerHTML = "<p>Please enter a numeric value for Dividends, Interest, Capital Gains from Your Investments and Other Annuities in the Monthly Income section.</p>";    
			}
			
			else {
				
				investmentTemp = parseFloat(myTemp);
				
				 if (investmentTemp<0){
					document.getElementById("investmentError").style.display = "block";
					document.getElementById("investmentError").innerHTML = "<p>Please enter a positive number.</p>";
					investmentTemp = 0;
					
					document.getElementById("investmentError1").style.display = "block";
					document.getElementById("investmentError1").innerHTML = "<p>Please enter a positive number for Dividends, Interest, Capital Gains from Your Investments and Other Annuities in the Monthly Income section.</p>";    
				}
				
				else if (investmentTemp>10000000){
					document.getElementById("investmentError").style.display = "block";
					document.getElementById("investmentError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					investmentTemp = 0;
					
					document.getElementById("investmentError1").style.display = "block";
					document.getElementById("investmentError1").innerHTML = "<p>The value for Dividends, Interest, Capital Gains from Your Investments and Other Annuities in the Monthly Income section must be less than 10,000,001.</p>";    
				}
				else {
					document.getElementById("investmentError").style.display = "none";
					document.getElementById("investment").value = numberWithCommas(investmentTemp.toFixed(0));
					document.getElementById("investmentError1").style.display = "none";
				}
			}
			
		}
		calculateIncome();
	}
	
	function addCompanyPension() {
		
		var myTemp = trimInput(document.getElementById("companyPension").value); 
		
		if (myTemp =="")
		{
			companyPensionTemp = 0; 
			document.getElementById("companyPensionError").style.display = "none";
			document.getElementById("companyPension").value = "0";
			document.getElementById("companyPensionError1").style.display = "none";
			
		}
		else {
		
		if(isNaN(myTemp)){
			document.getElementById("companyPensionError").style.display = "block";
			document.getElementById("companyPensionError").innerHTML = "<p>Please enter a numeric value.</p>";
			companyPensionTemp = 0; 
			
			document.getElementById("companyPensionError1").style.display = "block";
			document.getElementById("companyPensionError1").innerHTML = "<p>Please enter a numeric value for Company Pension Income in the Monthly Income section.</p>";    
		}
		
		else {
			
			companyPensionTemp = parseFloat(myTemp);
			
			if (companyPensionTemp<0){
				document.getElementById("companyPensionError").style.display = "block";
				document.getElementById("companyPensionError").innerHTML = "<p>Please enter a positive number.</p>";
				companyPensionTemp = 0; 
				
				document.getElementById("companyPensionError1").style.display = "block";
				document.getElementById("companyPensionError1").innerHTML = "<p>Please enter a positive number for Company Pension Income in the Monthly Income section.</p>";    
			}
			
			else if (companyPensionTemp>10000000){
				document.getElementById("companyPensionError").style.display = "block";
				document.getElementById("companyPensionError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
				companyPensionTemp = 0; 
				
				document.getElementById("companyPensionError1").style.display = "block";
				document.getElementById("companyPensionError1").innerHTML = "<p>The value for Company Pension Income in the Monthly Income section must be less than 10,000,001.</p>";    
			}
			else {
				document.getElementById("companyPensionError").style.display = "none";
				document.getElementById("companyPension").value = numberWithCommas(companyPensionTemp.toFixed(0));
				document.getElementById("companyPensionError1").style.display = "none";
				
			}
		}
		}
		calculateIncome();
	}
	
	function addEmployment() {
		
		var myTemp = trimInput(document.getElementById("employment").value); 
		
		if (myTemp =="")
		{
			employmentTemp = 0; 
			document.getElementById("employmentError").style.display = "none";
			document.getElementById("employment").value = "0";
			document.getElementById("employmentError1").style.display = "none";
			
		}
		else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("employmentError").style.display = "block";
				document.getElementById("employmentError").innerHTML = "<p>Please enter a numeric value.</p>";
				employmentTemp = 0; 
				
				document.getElementById("employmentError1").style.display = "block";
				document.getElementById("employmentError1").innerHTML = "<p>Please enter a numeric value for Employment Income in the Monthly Income section.</p>";    
			}
			
			else {
				
				employmentTemp = parseFloat(myTemp);
				
				if (employmentTemp<0){
					document.getElementById("employmentError").style.display = "block";
					document.getElementById("employmentError").innerHTML = "<p>Please enter a positive number.</p>";
					employmentTemp = 0; 
					
					document.getElementById("employmentError1").style.display = "block";
					document.getElementById("employmentError1").innerHTML = "<p>Please enter a positive number for Employment Income in the Monthly Income section.</p>";    
				}
				
				else if (employmentTemp>10000000){
					document.getElementById("employmentError").style.display = "block";
					document.getElementById("employmentError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					employmentTemp = 0; 
					
					document.getElementById("employmentError1").style.display = "block";
					document.getElementById("employmentError1").innerHTML = "<p>The value for Employment Income in the Monthly Income section must be less than 10,000,001.</p>";    
				}
				else {
					document.getElementById("employmentError").style.display = "none";
					document.getElementById("employment").value = numberWithCommas(employmentTemp.toFixed(0));
					document.getElementById("employmentError1").style.display = "none";
				}
			}
		}
		
		calculateIncome();
	}
	
	function addOther() {
		
		var myTemp = trimInput(document.getElementById("other").value); 
		
		if (myTemp =="")
		{
			otherTemp = 0; 
			document.getElementById("otherIncomeError").style.display = "none";
			document.getElementById("other").value = "0";
			document.getElementById("otherIncomeError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("otherIncomeError").style.display = "block";
				document.getElementById("otherIncomeError").innerHTML = "<p>Please enter a numeric value.</p>";
				otherTemp = 0; 
				
				document.getElementById("otherIncomeError1").style.display = "block";
				document.getElementById("otherIncomeError1").innerHTML = "<p>Please enter a numeric value for Other Source of Income in the Monthly Income section.</p>";    
			}
			
			else {
				
				
				otherTemp = parseFloat(myTemp);
				
				if (otherTemp<0){
					document.getElementById("otherIncomeError").style.display = "block";
					document.getElementById("otherIncomeError").innerHTML = "<p>Please enter a positive number.</p>";
					otherTemp = 0; 
					
					document.getElementById("otherIncomeError1").style.display = "block";
					document.getElementById("otherIncomeError1").innerHTML = "<p>Please enter a positive number for Other Source of Income in the Monthly Income section.</p>"; 
				}
				
				else if (otherTemp>10000000){
					document.getElementById("otherIncomeError").style.display = "block";
					document.getElementById("otherIncomeError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					otherTemp = 0; 
					
					document.getElementById("otherIncomeError1").style.display = "block";
					document.getElementById("otherIncomeError1").innerHTML = "<p>The value for Other Source of Income in the Monthly Income section must be less than 10,000,001.</p>"; 
				}
				else {
					document.getElementById("otherIncomeError").style.display = "none";
					document.getElementById("other").value = numberWithCommas(otherTemp.toFixed(0));
					document.getElementById("otherIncomeError1").style.display = "none";
				}
			}
		}
		calculateIncome();
	}
	
	function addRental() {
		
		var myTemp = trimInput(document.getElementById("rental").value); 
		
		if (myTemp =="")
		{
			rentalTemp = 0; 
			document.getElementById("rentalError").style.display = "none";
			document.getElementById("rental").value = "0";
			document.getElementById("rentalError1").style.display = "none";
			
		}
		else {
			
		
			
			if(isNaN(myTemp)){
				document.getElementById("rentalError").style.display = "block";
				document.getElementById("rentalError").innerHTML = "<p>Please enter a numeric value.</p>";
				rentalTemp = 0;
				
				document.getElementById("rentalError1").style.display = "block";
				document.getElementById("rentalError1").innerHTML = "<p>Please enter a numeric value for Rental Income in the Monthly Income section.</p>";    
			}
			
			else {
				
				rentalTemp = parseFloat(myTemp);
					
				if (rentalTemp<0){
					document.getElementById("rentalError").style.display = "block";
					document.getElementById("rentalError").innerHTML = "<p>Please enter a positive number.</p>";
					rentalTemp = 0; 
					
					document.getElementById("rentalError1").style.display = "block";
					document.getElementById("rentalError1").innerHTML = "<p>Please enter a positive number for Rental Income in the Monthly Income section.</p>"; 
				}
				else if (rentalTemp>10000000){
					document.getElementById("rentalError").style.display = "block";
					document.getElementById("rentalError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					rentalTemp = 0; 
					
					document.getElementById("rentalError1").style.display = "block";
					document.getElementById("rentalError1").innerHTML = "<p>The value for Rental Income in the Monthly Income section must be less than 10,000,001.</p>"; 
				}
				else {
					document.getElementById("rentalError").style.display = "none";
					document.getElementById("rental").value = numberWithCommas(rentalTemp.toFixed(0));
					document.getElementById("rentalError1").style.display = "none";
				}
			}
		}
		calculateIncome();
	}
	
	function addOtherIncome(){
		
		var myTemp = trimInput(document.getElementById("otherIncome").value); 
		
		if (myTemp =="")
		{
			otherIncomeTemp = 0; 
			document.getElementById("otherIncome2Error").style.display = "none";
			document.getElementById("otherIncome").value = "0";
			document.getElementById("otherIncome2Error1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("otherIncome2Error").style.display = "block";
				document.getElementById("otherIncome2Error").innerHTML = "<p>Please enter a numeric value.</p>";
				otherIncomeTemp = 0; 
				
				document.getElementById("otherIncome2Error1").style.display = "block";
				document.getElementById("otherIncome2Error1").innerHTML = "<p>Please enter a numeric value for Other in the Monthly Income section.</p>"; 
			}
			
			else {
				
				otherIncomeTemp = parseFloat(myTemp);
				
				if (otherIncomeTemp<0){
					document.getElementById("otherIncome2Error").style.display = "block";
					document.getElementById("otherIncome2Error").innerHTML = "<p>Please enter a positive number.</p>";
					otherIncomeTemp = 0;
					
					document.getElementById("otherIncome2Error1").style.display = "block";
					document.getElementById("otherIncome2Error1").innerHTML = "<p>Please enter a positive number for Other in the Monthly Income section.</p>";  
				}
				else if (otherIncomeTemp>10000000){
					document.getElementById("otherIncome2Error").style.display = "block";
					document.getElementById("otherIncome2Error").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					otherIncomeTemp = 0;
					
					document.getElementById("otherIncome2Error1").style.display = "block";
					document.getElementById("otherIncome2Error1").innerHTML = "<p>The value for Other in the Monthly Income section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("otherIncome2Error").style.display = "none";
					document.getElementById("otherIncome").value = numberWithCommas(otherIncomeTemp.toFixed(0));
					document.getElementById("otherIncome2Error1").style.display = "none";
				}
			}
		}
		calculateIncome();
	}
	
	function updateHomeValue() {
		
		var myTemp = trimInput(document.getElementById("homeValue").value); 
		
		if (myTemp =="")
		{
			homeValue = 0; 
			document.getElementById("homeError1").style.display = "block";
			document.getElementById("homeError1").innerHTML = "<p>The value must be between 25,000 and 25,000,000.</p>";
			document.getElementById("homeValue").value = "0";
			document.getElementById("homeError2").style.display = "block";
			document.getElementById("homeError2").innerHTML = "<p>The Estimated Value of Your Home must be between 25,000 and 25,000,000 in the Home Equity section.</p>";
			document.getElementById("rateOfReturn").style.display = "none";
			document.getElementById("defaultDisplay").selected = "true";
			document.getElementById("own").style.display ="none"; 
			rateOfReturn = 0;
			
		}
		
		else {
			
			if (isNaN(myTemp)){
				document.getElementById("homeError1").style.display = "block";
				document.getElementById("homeError1").innerHTML = "<p>Please enter a numeric value.</p>";
				homeValue = 0; 
				
				document.getElementById("homeError2").style.display = "block";
				document.getElementById("homeError2").innerHTML = "<p>Please enter a numeric value for Estimated Value of Your Home in the Home Equity section.</p>";
				document.getElementById("rateOfReturn").style.display = "none";
				document.getElementById("defaultDisplay").selected = "true";
				rateOfReturn = 0;
				document.getElementById("own").style.display ="none"; 
			}
			
			else {
				
				homeValue = parseFloat(myTemp);
				
				if (homeValue<25000 || homeValue> 25000000){
					document.getElementById("homeError1").style.display = "block";
					document.getElementById("homeError1").innerHTML = "<p>The value must be between 25,000 and 25,000,000.</p>";
					homeValue = 0; 
					document.getElementById("homeError2").style.display = "block";
					document.getElementById("homeError2").innerHTML = "<p>The Estimated Value of Your Home must be between 25,000 and 25,000,000 in the Home Equity section.</p>";
					document.getElementById("rateOfReturn").style.display = "none";
					document.getElementById("defaultDisplay").selected = "true";
					rateOfReturn = 0;
					document.getElementById("own").style.display ="none"; 
				}
				
				else if (homeValue<0){
					document.getElementById("homeError1").style.display = "block";
					document.getElementById("homeError1").innerHTML = "<p>Please enter a positive number.</p>";
					homeValue = 0; 
					
				   document.getElementById("homeError2").style.display = "block";
				   document.getElementById("homeError2").innerHTML = "<p>Please enter a positive number for Estimated Value of Your Home in the Home Equity section.</p>";
				   document.getElementById("rateOfReturn").style.display = "none";
				   document.getElementById("defaultDisplay").selected = "true";
				   rateOfReturn = 0;
				   document.getElementById("own").style.display ="none"; 
				}
				
				else {
				document.getElementById("homeError1").style.display = "none";
				document.getElementById("rateOfReturn").style.display = "block";
				document.getElementById("homeValue").value = numberWithCommas(homeValue.toFixed(0));
				document.getElementById("homeError2").style.display = "none";
				}
			}
		}
		
		calculateAdditionalIncome ();
	}
	
	function updateMortgage() {
		
		var myTemp = trimInputLess(document.getElementById("mortgage").value); 
		
		if (myTemp =="")
		{
			mortgage = 0; 
			document.getElementById("mortgageError").style.display = "none";
			document.getElementById("mortgage").value = "0";
			document.getElementById("mortgageError1").style.display = "none";
			
		}
		
		else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("mortgageError").style.display = "block";
				document.getElementById("mortgageError").innerHTML = "<p>Please enter a numeric value.</p>";
				mortgage = 0; 
				
				document.getElementById("mortgageError1").style.display = "block";
				document.getElementById("mortgageError1").innerHTML = "<p>Please enter a numeric value for Outstanding Mortgage in the Home Equity section.</p>";
			}
			
			else {
				
				mortgage = parseFloat(myTemp);
				
				if (mortgage<0){
					document.getElementById("mortgageError").style.display = "block";
					document.getElementById("mortgageError").innerHTML = "<p>Please enter a positive number.</p>";
					mortgage = 0; 
					
					document.getElementById("mortgageError1").style.display = "block";
					document.getElementById("mortgageError1").innerHTML = "<p>Please enter a positive number for Outstanding Mortgage in the Home Equity section.</p>";
				}
				
				else if (mortgage>10000000){
					document.getElementById("mortgageError").style.display = "block";
					document.getElementById("mortgageError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					mortgage = 0; 
					
					document.getElementById("mortgageError1").style.display = "block";
					document.getElementById("mortgageError1").innerHTML = "<p>The value for Outstanding Mortgage in the Home Equity section must be less than 10,000,001.</p>";
				}
				else {
					/*if (mortgage>homeValue){
						document.getElementById("mortgageError").style.display = "block";
						document.getElementById("mortgageError").innerHTML = "<p>The mortgage must be less than "+homeValue+"</p>";
						document.getElementById("mortgage").value = "";
					}
					else {*/
						document.getElementById("mortgageError").style.display = "none";
						document.getElementById("mortgage").value = "("+numberWithCommas(mortgage.toFixed(0))+")";
						document.getElementById("mortgageError1").style.display = "none";
					//}
				}
			}
		}
		calculateAdditionalIncome ();
	}
	
	function updateLegal() {
		
		var myTemp = trimInputLess(document.getElementById("legal").value); 
		
		
		if (myTemp =="")
		{
			legal = 0; 
			document.getElementById("legalError").style.display = "none";
			document.getElementById("legal").value = "0";
			document.getElementById("legalError1").style.display = "none";
			
		}
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("legalError").style.display = "block";
				document.getElementById("legalError").innerHTML = "<p>Please enter a numeric value.</p>";
				legal = 0; 
				
				document.getElementById("legalError1").style.display = "block";
				document.getElementById("legalError1").innerHTML = "<p>Please enter a numeric value for Legal Fees & Disbursements in the Home Equity section.</p>";
			}
			
			else {
				
				legal = parseFloat(myTemp);
				
				if(legal<0){
					document.getElementById("legalError").style.display = "block";
					document.getElementById("legalError").innerHTML = "<p>Please enter a positive number.</p>";
					legal = 0; 
					
					document.getElementById("legalError1").style.display = "block";
					document.getElementById("legalError1").innerHTML = "<p>Please enter a positive number for Legal Fees & Disbursements in the Home Equity section.</p>";
				}
				else if (legal>10000000){
					document.getElementById("legalError").style.display = "block";
					document.getElementById("legalError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					legal = 0; 
					
					document.getElementById("legalError1").style.display = "block";
					document.getElementById("legalError1").innerHTML = "<p>The value for Legal Fees & Disbursements in the Home Equity section must be less than 10,000,001.</p>";
				}
				else {
					document.getElementById("legalError").style.display = "none";
					document.getElementById("legal").value = "("+numberWithCommas(legal.toFixed(0))+ ")";
					document.getElementById("legalError1").style.display = "none";
				}
			}
		}
			calculateAdditionalIncome ();
	}
	
	function updateCommission() {
		
		var myTemp = trimInputLess(document.getElementById("commission").value); 
		
		if (myTemp =="")
		{
			commission = 0; 
			document.getElementById("commissionError").style.display = "none";
			document.getElementById("commission").value = "0";
			document.getElementById("commissionError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("commissionError").style.display = "block";
				document.getElementById("commissionError").innerHTML = "<p>Please enter a numeric value.</p>";
				commission = 0;
				
				document.getElementById("commissionError1").style.display = "block";
				document.getElementById("commissionError1").innerHTML = "<p>Please enter a numeric value for Realtor Commission Fees in the Home Equity section.</p>"; 
			}
			
			else {
				
				commission = parseFloat(myTemp);
				
				if (commission<0){
					document.getElementById("commissionError").style.display = "block";
					document.getElementById("commissionError").innerHTML = "<p>Please enter a positive number.</p>";
					commission = 0;
					
					document.getElementById("commissionError1").style.display = "block";
					document.getElementById("commissionError1").innerHTML = "<p>Please enter a positive number for Realtor Commission Fees in the Home Equity section.</p>";  
				}
				
				else if (commission>10000000){
					document.getElementById("commissionError").style.display = "block";
					document.getElementById("commissionError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					commission = 0;
					
					document.getElementById("commissionError1").style.display = "block";
					document.getElementById("commissionError1").innerHTML = "<p>The value for Realtor Commission Fees in the Home Equity section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("commissionError").style.display = "none";
					document.getElementById("commission").value = "("+numberWithCommas(commission.toFixed(0))+")";
					document.getElementById("commissionError1").style.display = "none";
				}
			}
		}
		
		calculateAdditionalIncome ();
	}
	
	function updateMovingCosts() {
		
		var myTemp = trimInputLess(document.getElementById("movingCosts").value);  
	
		if (myTemp =="")
		{
			movingCosts = 0; 
			document.getElementById("movingCostsError").style.display = "none";
			document.getElementById("movingCosts").value = "0";
			document.getElementById("movingCostsError1").style.display = "none";
		
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("movingCostsError").style.display = "block";
				document.getElementById("movingCostsError").innerHTML = "<p>Please enter a numeric value.</p>";
				movingCosts = 0;
				
				document.getElementById("movingCostsError1").style.display = "block";
				document.getElementById("movingCostsError1").innerHTML = "<p>Please enter a numeric value for Moving and Other Costs in the Home Equity section.</p>";  
			}
			
			else {
				
				movingCosts = parseFloat(myTemp);
			
				if (movingCosts<0){
					document.getElementById("movingCostsError").style.display = "block";
					document.getElementById("movingCostsError").innerHTML = "<p>Please enter a positive number.</p>";
					movingCosts = 0;
					
					document.getElementById("movingCostsError1").style.display = "block";
					document.getElementById("movingCostsError1").innerHTML = "<p>Please enter a positive number for Moving and Other Costs in the Home Equity section.</p>";  
				}
				
				else if (movingCosts>10000000){
					document.getElementById("movingCostsError").style.display = "block";
					document.getElementById("movingCostsError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					movingCosts = 0;
					
					document.getElementById("movingCostsError1").style.display = "block";
					document.getElementById("movingCostsError1").innerHTML = "<p>The value for Moving and Other Costs in the Home Equity section must be less than 10,000,001.</p>";  
				}
				
				else {
					document.getElementById("movingCostsError").style.display = "none";
					document.getElementById("movingCosts").value = "("+numberWithCommas(movingCosts.toFixed(0))+")";
					
					document.getElementById("movingCostsError1").style.display = "none";
					
				}
			}
		}
		
		calculateAdditionalIncome ();
		
	}
	
	function updateRateOfReturn() {
		var currentRate = document.getElementById("rate").value;
		document.getElementById("rateError1").style.display = "none";
		document.getElementById("rateError").style.display = "none";
		
		if (currentRate != "ownRate"){
			rateOfReturn = parseFloat(currentRate);
			document.getElementById("own").style.display = "none";
			document.getElementById("ownRate").value = "0";
			
			
		}
		else {
			
			document.getElementById("own").style.display = "block";
			rateOfReturn = 0;
			document.getElementById("ownRate").value = "0";
		}
		calculateCashProceed ();
		calculateHomeEquity();
		
	}
	
	function getOwnRate (){
		
		var myTemp = trimInput(document.getElementById("ownRate").value); 
		
		if (myTemp =="")
		{
			rateOfReturn = 0; 
			document.getElementById("rateError").style.display = "none";
			document.getElementById("ownRate").value = "0";
			document.getElementById("rateError1").style.display = "none";
			calculateCashProceed ();
			calculateHomeEquity();  
			
		}
		
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("rateError").style.display = "block";
				document.getElementById("rateError").innerHTML = "<p>Please enter a numeric value.</p>";
				rateOfReturn = 0; 
				
				document.getElementById("rateError1").style.display = "block";
				document.getElementById("rateError1").innerHTML = "<p>Please enter a numeric value for Your Own Estimated Rate of Return in the Home Equity section.</p>";
				calculateCashProceed ();
				calculateHomeEquity();  
				
			}
			
			else {
				
				rateOfReturn = parseFloat(myTemp)*0.01;
			   
				if (rateOfReturn<0){
					document.getElementById("rateError").style.display = "block";
					document.getElementById("rateError").innerHTML = "<p>Please enter a positive number.</p>";
					rateOfReturn = 0; 
					
					document.getElementById("rateError1").style.display = "block";
					document.getElementById("rateError1").innerHTML = "<p>Please enter a positive number for Your Own Estimated Rate of Return in the Home Equity section.</p>";
					calculateCashProceed ();
					calculateHomeEquity();  
				}
				
				else if (rateOfReturn>0.3){
					document.getElementById("rateError").style.display = "block";
					document.getElementById("rateError").innerHTML = "<p>The rate of return must be less than or equal to 30%.</p>";
					rateOfReturn = 0; 
					
					document.getElementById("rateError1").style.display = "block";
					document.getElementById("rateError1").innerHTML = "<p>Your Own Estimated Rate of Return in the Home Equity Section must be less than or equal to 30%.</p>";  
				    calculateCashProceed ();
					calculateHomeEquity();  
				}
				
				else {
					document.getElementById("rateError").style.display = "none";
					document.getElementById("rateError1").style.display = "none";
					document.getElementById("ownRate").value = numberWithCommas(myTemp);
					calculateCashProceed ();
					calculateHomeEquity();
				}
			}
		}
		
	}
	
	function updateMortgageCost (){
		
		var myTemp = trimInput(document.getElementById("mortgageCost").value); 
		
		if (myTemp =="")
		{
			mortgageCost = 0; 
			document.getElementById("mortgageCostError").style.display = "none";
			document.getElementById("mortgageCost").value = "0";
			document.getElementById("mortgageCostError1").style.display = "none";
			
		}
		
		else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("mortgageCostError").style.display = "block";
				document.getElementById("mortgageCostError").innerHTML = "<p>Please enter a numeric value.</p>";
				mortgageCost = 0; 
				
				document.getElementById("mortgageCostError1").style.display = "block";
				document.getElementById("mortgageCostError1").innerHTML = "<p>Please enter a numeric value for Mortgage in the Housing Expenses section.</p>";  
			}
			
			else {
				
				mortgageCost = parseFloat(myTemp);
				
				if (mortgageCost<0){
					document.getElementById("mortgageCostError").style.display = "block";
					document.getElementById("mortgageCostError").innerHTML = "<p>Please enter a positive number.</p>";
					mortgageCost = 0;
					
					document.getElementById("mortgageCostError1").style.display = "block";
					document.getElementById("mortgageCostError1").innerHTML = "<p>Please enter a positive number for Mortgage in the Housing Expenses section.</p>";   
				}
				
				else if (mortgageCost>10000000){
					document.getElementById("mortgageCostError").style.display = "block";
					document.getElementById("mortgageCostError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					mortgageCost = 0;
					
					document.getElementById("mortgageCostError1").style.display = "block";
					document.getElementById("mortgageCostError1").innerHTML = "<p>The value for Mortgage in the Housing Expenses section must be less than 10,000,001.</p>";   
				}
				
				else {
					document.getElementById("mortgageCostError").style.display = "none";
					document.getElementById("mortgageCost").value = numberWithCommas(mortgageCost.toFixed(0));
					document.getElementById("mortgageCostError1").style.display = "none";
				}
			}
		}
		calculateHousingExpenses();
	}
	
	function updateRent (){
		
		var myTemp = trimInput(document.getElementById("rent").value); 
		
		if (myTemp==""){
			rent = 0; 
			document.getElementById("rentError").style.display = "none";
			document.getElementById("rent").value = "0";
			document.getElementById("rentError1").style.display = "none";
		}
		
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("rentError").style.display = "block";
				document.getElementById("rentError").innerHTML = "<p>Please enter a numeric value.</p>";
				rent = 0;
				
				document.getElementById("rentError1").style.display = "block";
				document.getElementById("rentError1").innerHTML = "<p>Please enter a numeric value for Rent in the Housing Expenses section.</p>";   
			}
			
			else {
				
				rent = parseFloat(myTemp);
				
				if (rent<0){
					document.getElementById("rentError").style.display = "block";
					document.getElementById("rentError").innerHTML = "<p>Please enter a positive number.</p>";
					rent = 0; 
					
					document.getElementById("rentError1").style.display = "block";
					document.getElementById("rentError1").innerHTML = "<p>Please enter a positive number for Rent in the Housing Expenses section.</p>";   
				}
				
				else if (rent>10000000){
					document.getElementById("rentError").style.display = "block";
					document.getElementById("rentError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					rent = 0; 
					
					document.getElementById("rentError1").style.display = "block";
					document.getElementById("rentError1").innerHTML = "<p>The value for Rent in the Housing Expenses section must be less than 10,000,001.</p>";   
				}
				
				else {
					document.getElementById("rentError").style.display = "none";
					document.getElementById("rent").value = numberWithCommas(rent.toFixed(0));
					document.getElementById("rentError1").style.display = "none";
					
				}
			}
		}
		calculateHousingExpenses();
	}
	
	function updateCondo (){
		
		var myTemp = trimInput(document.getElementById("condo").value);
		
		if (myTemp==""){
			condo = 0; 
			document.getElementById("condoError").style.display = "none";
			document.getElementById("condo").value = "0";
			document.getElementById("condoError1").style.display = "none";
		}
		
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("condoError").style.display = "block";
				document.getElementById("condoError").innerHTML = "<p>Please enter a numeric value.</p>";
				condo = 0;
				
				document.getElementById("condoError1").style.display = "block";
				document.getElementById("condoError1").innerHTML = "<p>Please enter a numeric value for Condo Fees in the Housing Expenses section.</p>";    
			}
			
			else {
				
				condo = parseFloat(myTemp);
				
				if (condo<0){
					document.getElementById("condoError").style.display = "block";
					document.getElementById("condoError").innerHTML = "<p>Please enter a positive number.</p>";
					condo = 0; 
					
					document.getElementById("condoError1").style.display = "block";
					document.getElementById("condoError1").innerHTML = "<p>Please enter a positive number for Condo Fees in the Housing Expenses section.</p>";  
				}
				else if (condo>10000000){
					document.getElementById("condoError").style.display = "block";
					document.getElementById("condoError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					condo = 0; 
					
					document.getElementById("condoError1").style.display = "block";
					document.getElementById("condoError1").innerHTML = "<p>The value for Condo Fees in the Housing Expenses section must be less than 10,000,001.</p>";  
				}
				
				
				else {
					document.getElementById("condoError").style.display = "none";
					document.getElementById("condo").value = numberWithCommas(condo.toFixed(0));
					document.getElementById("condoError1").style.display = "none";
					
				}
			}
		}
		calculateHousingExpenses();
	}
	
	function updatePropertyTaxes (){
		
		var myTemp = trimInput(document.getElementById("propertyTaxes").value); 
		
		if (myTemp =="")
		{
			propertyTaxes = 0; 
			document.getElementById("propertyTaxesError").style.display = "none";
			document.getElementById("propertyTaxes").value = "0";
			document.getElementById("propertyTaxesError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("propertyTaxesError").style.display = "block";
				document.getElementById("propertyTaxesError").innerHTML = "<p>Please enter a numeric value.</p>";
				propertyTaxes = 0;
				
				document.getElementById("propertyTaxesError1").style.display = "block";
				document.getElementById("propertyTaxesError1").innerHTML = "<p>Please enter a numeric value for Property Taxes in the Housing Expenses section.</p>";
				 
			}
			
			else {
				
				propertyTaxes = parseFloat(myTemp);
				
				if (propertyTaxes<0){
					document.getElementById("propertyTaxesError").style.display = "block";
					document.getElementById("propertyTaxesError").innerHTML = "<p>Please enter a positive number.</p>";
					propertyTaxes = 0; 
					
					document.getElementById("propertyTaxesError1").style.display = "block";
					document.getElementById("propertyTaxesError1").innerHTML = "<p>Please enter a positive number for Property Taxes in the Housing Expenses section.</p>";
				}
				
				else if (propertyTaxes>10000000){
					document.getElementById("propertyTaxesError").style.display = "block";
					document.getElementById("propertyTaxesError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					propertyTaxes = 0; 
					
					document.getElementById("propertyTaxesError1").style.display = "block";
					document.getElementById("propertyTaxesError1").innerHTML = "<p>The value for Property Taxes in the Housing Expenses section must be less than 10,000,001.</p>";
				}
				else {
					document.getElementById("propertyTaxesError").style.display = "none";
					document.getElementById("propertyTaxes").value = numberWithCommas(propertyTaxes.toFixed(0));
					document.getElementById("propertyTaxesError1").style.display = "none";
					
				}
			}
		}
		calculateHousingExpenses();
	}
	
	function updateUtilitiesGas (){
		
		var myTemp = trimInput(document.getElementById("utilitiesGas").value)
		
		if (myTemp =="")
		{
			utilitiesGas = 0; 
			document.getElementById("gasError").style.display = "none";
			document.getElementById("utilitiesGas").value = "0";
			document.getElementById("gasError1").style.display = "none";
			
		}
		
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("gasError").style.display = "block";
				document.getElementById("gasError").innerHTML = "<p>Please enter a numeric value.</p>";
				utilitiesGas = 0; 
				
				document.getElementById("gasError1").style.display = "block";
				document.getElementById("gasError1").innerHTML = "<p>Please enter a numeric value for Gas/Fuel Oil in the Housing Expenses section.</p>";
			}
			
			else {
				
				utilitiesGas = parseFloat(myTemp);
				
				if (utilitiesGas<0){
					document.getElementById("gasError").style.display = "block";
					document.getElementById("gasError").innerHTML = "<p>Please enter a positive number.</p>";
					utilitiesGas = 0; 
					
					document.getElementById("gasError1").style.display = "block";
					document.getElementById("gasError1").innerHTML = "<p>Please enter a positive number for Gas/Fuel Oil in the Housing Expenses section.</p>";
				}
				
				else if (utilitiesGas>10000000){
					document.getElementById("gasError").style.display = "block";
					document.getElementById("gasError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					utilitiesGas = 0; 
					
					document.getElementById("gasError1").style.display = "block";
					document.getElementById("gasError1").innerHTML = "<p>The value for Gas/Fuel Oil in the Housing Expenses section must be less than 10,000,001.</p>";
				}
				else {
					document.getElementById("gasError").style.display = "none";
					document.getElementById("utilitiesGas").value = numberWithCommas(utilitiesGas.toFixed(0));
					document.getElementById("gasError1").style.display = "none";
				}
			}
		}
		
		calculateHousingExpenses();
	}
	
	function updateUtilitiesElectricity (){
		
		var myTemp = trimInput(document.getElementById("utilitiesElectricity").value);
		
		
		if (myTemp =="")
		{
			utilitiesElectricity = 0; 
			document.getElementById("electricityError").style.display = "none";
			document.getElementById("utilitiesElectricity").value = "0";
			document.getElementById("electricityError1").style.display = "none";
			
		}
		else {
		
			
			
			if(isNaN(myTemp)){
				
				document.getElementById("electricityError").style.display = "block";
				document.getElementById("electricityError").innerHTML = "<p>Please enter a numeric value.</p>";
				utilitiesElectricity = 0;
				
				document.getElementById("electricityError1").style.display = "block";
				document.getElementById("electricityError1").innerHTML = "<p>Please enter a numeric value for Electricity in the Housing Expenses section.</p>"; 
			}
			
			else {
				
				utilitiesElectricity = parseFloat(myTemp);
				
				if (utilitiesElectricity<0){
					document.getElementById("electricityError").style.display = "block";
					document.getElementById("electricityError").innerHTML = "<p>Please enter a positive number.</p>";
					utilitiesElectricity = 0; 
					
					document.getElementById("electricityError1").style.display = "block";
					document.getElementById("electricityError1").innerHTML = "<p>Please enter a positive number for Electricity in the Housing Expenses section.</p>"; 
				}
				
				else if (utilitiesElectricity>10000000){
					document.getElementById("electricityError").style.display = "block";
					document.getElementById("electricityError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					utilitiesElectricity = 0; 
					
					document.getElementById("electricityError1").style.display = "block";
					document.getElementById("electricityError1").innerHTML = "<p>The value for Electricity in the Housing Expenses section must be less than 10,000,001.</p>"; 
				}
				
				else {
					document.getElementById("electricityError").style.display = "none";
					document.getElementById("utilitiesElectricity").value = numberWithCommas(utilitiesElectricity.toFixed(0));
					document.getElementById("electricityError1").style.display = "none";
				}
			}
		}
			calculateHousingExpenses();
	}
	
	function updateOtherUtilities (){
		
		var myTemp = trimInput(document.getElementById("otherUtilities").value); 
		
		
		if (myTemp =="")
		{
			otherUtilities = 0; 
			document.getElementById("otherUtlitiesError").style.display = "none";
			document.getElementById("otherUtilities").value = "0";
			document.getElementById("otherUtlitiesError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("otherUtlitiesError").style.display = "block";
				document.getElementById("otherUtlitiesError").innerHTML = "<p>Please enter a numeric value.</p>";
				otherUtilities = 0;
				
				document.getElementById("otherUtlitiesError1").style.display = "block";
				document.getElementById("otherUtlitiesError1").innerHTML = "<p>Please enter a numeric value for Water and Other Utilities in the Housing Expenses section.</p>";  
			}
			
			else {
				
				
				otherUtilities = parseFloat(myTemp);
				
				if (otherUtilities<0){

					document.getElementById("otherUtlitiesError").style.display = "block";
					document.getElementById("otherUtlitiesError").innerHTML = "<p>Please enter a positive number.</p>";
					otherUtilities = 0;
					
					document.getElementById("otherUtlitiesError1").style.display = "block";
					document.getElementById("otherUtlitiesError1").innerHTML = "<p>Please enter a positive number for Water and Other Utilities in the Housing Expenses section.</p>";  
				}
				
				else if (otherUtilities>10000000){
					document.getElementById("otherUtlitiesError").style.display = "block";
					document.getElementById("otherUtlitiesError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					otherUtilities = 0;
					
					document.getElementById("otherUtlitiesError1").style.display = "block";
					document.getElementById("otherUtlitiesError1").innerHTML = "<p>The value for Water and Other Utilities in the Housing Expenses section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("otherUtlitiesError").style.display = "none";
					document.getElementById("otherUtilities").value = numberWithCommas(otherUtilities.toFixed(0));
					document.getElementById("otherUtlitiesError1").style.display = "none";
				}
			}
		}
		calculateHousingExpenses();
	}
	
	function updatePremium (){
		
		var myTemp = trimInput(document.getElementById("premium").value); 
	
		if (myTemp =="")
		{
			premium = 0; 
			document.getElementById("premiumError").style.display = "none";
			document.getElementById("premium").value = "0";
			document.getElementById("premiumError1").style.display = "none";
		}
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("premiumError").style.display = "block";
				document.getElementById("premiumError").innerHTML = "<p>Please enter a numeric value.</p>";
				premium = 0;
				
				document.getElementById("premiumError1").style.display = "block";
				document.getElementById("premiumError1").innerHTML = "<p>Please enter a numeric value for Home Insurance Premium in the Housing Expenses section.</p>";   
			}
			
			else {
				
				premium = parseFloat(myTemp);
				
				if (premium<0){
					document.getElementById("premiumError").style.display = "block";
					document.getElementById("premiumError").innerHTML = "<p>Please enter a positive number.</p>";
					premium = 0; 
					
					document.getElementById("premiumError1").style.display = "block";
					document.getElementById("premiumError1").innerHTML = "<p>Please enter a positive number for Home Insurance Premium in the Housing Expenses section.</p>";   
				}
				
				else if (premium>10000000){
					document.getElementById("premiumError").style.display = "block";
					document.getElementById("premiumError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					premium = 0; 
					
					document.getElementById("premiumError1").style.display = "block";
					document.getElementById("premiumError1").innerHTML = "<p>The value for Home Insurance Premium in the Housing Expenses section must be less than 10,000,001.</p>";   
				}
				
				else {
					document.getElementById("premiumError").style.display = "none";
					document.getElementById("premium").value = numberWithCommas(premium.toFixed(0));
					document.getElementById("premiumError1").style.display = "none";
				}
			}
		}
		calculateInsuranceExpenses();
	}
	
	function updateLawnCare (){
		
		var myTemp = trimInput(document.getElementById("lawnCare").value); 
		
		if (myTemp =="")
		{
			lawnCare = 0; 
			document.getElementById("lawnError").style.display = "none";
			document.getElementById("lawnCare").value = "0";
			document.getElementById("lawnError1").style.display = "none";
			
		}
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("lawnError").style.display = "block";
				document.getElementById("lawnError").innerHTML = "<p>Please enter a numeric value.</p>";
				lawnCare = 0;
				
				document.getElementById("lawnError1").style.display = "block";
				document.getElementById("lawnError1").innerHTML = "<p>Please enter a numeric value for Lawn Care in the Home Maintenance section.</p>";    
			}
			
			else {
				
				lawnCare = parseFloat(myTemp);
				
				if (lawnCare<0){
					document.getElementById("lawnError").style.display = "block";
					document.getElementById("lawnError").innerHTML = "<p>Please enter a positive number.</p>";
					lawnCare = 0;
					
					document.getElementById("lawnError1").style.display = "block";
					document.getElementById("lawnError1").innerHTML = "<p>Please enter a positive number for Lawn Care in the Home Maintenance section.</p>";    
				}
				
				else if (lawnCare>10000000){
					document.getElementById("lawnError").style.display = "block";
					document.getElementById("lawnError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					lawnCare = 0;
					
					document.getElementById("lawnError1").style.display = "block";
					document.getElementById("lawnError1").innerHTML = "<p>The value for Lawn Care in the Home Maintenance section must be less than 10,000,001.</p>";    
				}
				
				else {
					document.getElementById("lawnError").style.display = "none";
					document.getElementById("lawnCare").value = numberWithCommas(lawnCare.toFixed(0));
					document.getElementById("lawnError1").style.display = "none";
				}
			}
		
		}
			calculateHousingMaintenance();
	}
	
	function updateLandscaping (){
		
		var myTemp = trimInput(document.getElementById("landscaping").value); 
		
		if (myTemp =="")
		{
			landscaping = 0; 
			document.getElementById("landscapingError").style.display = "none";
			document.getElementById("landscaping").value = "0";
			document.getElementById("landscapingError1").style.display = "none";
			
		}
		
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("landscapingError").style.display = "block";
				document.getElementById("landscapingError").innerHTML = "<p>Please enter a numeric value.</p>";
				landscaping = 0;
				
				document.getElementById("landscapingError1").style.display = "block";
				document.getElementById("landscapingError1").innerHTML = "<p>Please enter a numeric value for Landscaping and Gardening in the Home Maintenance section.</p>";     
			}
			
			else {
				
				landscaping = parseFloat(myTemp);
				
				if (landscaping<0){
					document.getElementById("landscapingError").style.display = "block";
					document.getElementById("landscapingError").innerHTML = "<p>Please enter a positive number.</p>";
					landscaping = 0;
					
					document.getElementById("landscapingError1").style.display = "block";
					document.getElementById("landscapingError1").innerHTML = "<p>Please enter a positive number for Landscaping and Gardening in the Home Maintenance section.</p>";   
				}
				
				else if (landscaping>10000000){
					document.getElementById("landscapingError").style.display = "block";
					document.getElementById("landscapingError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					landscaping = 0;
					
					document.getElementById("landscapingError1").style.display = "block";
					document.getElementById("landscapingError1").innerHTML = "<p>The value for Landscaping and Gardening in the Home Maintenance section must be less than 10,000,001.</p>";   
				}
				
				else {
					document.getElementById("landscapingError").style.display = "none";
					document.getElementById("landscaping").value = numberWithCommas(landscaping.toFixed(0));
					document.getElementById("landscapingError1").style.display = "none";
				}
			}
		}
			calculateHousingMaintenance();
	}
	
	function updateSnowRemoval (){
		
		var myTemp = trimInput(document.getElementById("snowRemoval").value); 
		
		if (myTemp =="")
		{
			snowRemoval = 0; 
			document.getElementById("snowError").style.display = "none";
			document.getElementById("snowRemoval").value = "0";
			document.getElementById("snowError1").style.display = "none";
			
		}
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("snowError").style.display = "block";
				document.getElementById("snowError").innerHTML = "<p>Please enter a numeric value.</p>";
				snowRemoval = 0; 
				
				document.getElementById("snowError1").style.display = "block";
				document.getElementById("snowError1").innerHTML = "<p>Please enter a numeric value for Snow Removal in the Home Maintenance section.</p>";    
			}
			
			else {
				
				snowRemoval = parseFloat(myTemp);
				
				if (snowRemoval<0){
					document.getElementById("snowError").style.display = "block";
					document.getElementById("snowError").innerHTML = "<p>Please enter a positive number.</p>";
					snowRemoval = 0;
					
					document.getElementById("snowError1").style.display = "block";
					document.getElementById("snowError1").innerHTML = "<p>Please enter a positive number for Snow Removal in the Home Maintenance section.</p>";     
				}
				
				else if (snowRemoval>10000000){
					document.getElementById("snowError").style.display = "block";
					document.getElementById("snowError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					snowRemoval = 0;
					
					document.getElementById("snowError1").style.display = "block";
					document.getElementById("snowError1").innerHTML = "<p>The value for Snow Removal in the Home Maintenance section must be less than 10,000,001.</p>";     
				}
				
				else {
					document.getElementById("snowError").style.display = "none";
					document.getElementById("snowRemoval").value = numberWithCommas(snowRemoval.toFixed(0));
					document.getElementById("snowError1").style.display = "none";
				}
			}
		}
			calculateHousingMaintenance();
	}
	
	function updateSecurity(){
		
		var myTemp = trimInput(document.getElementById("security").value); 
		
		
		if (myTemp =="")
		{
			security = 0; 
			document.getElementById("securityError").style.display = "none";
			document.getElementById("security").value = "0";
			document.getElementById("securityError1").style.display = "none";
			
		}
		
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("securityError").style.display = "block";
				document.getElementById("securityError").innerHTML = "<p>Please enter a numeric value.</p>";
				security = 0; 
				
				document.getElementById("securityError1").style.display = "block";
				document.getElementById("securityError1").innerHTML = "<p>Please enter a numeric value for Security Monitoring Fees in the Home Maintenance section.</p>";    
				
			}
			
			else {
				
				security = parseFloat(myTemp);
				
				if (security<0){
					document.getElementById("securityError").style.display = "block";
					document.getElementById("securityError").innerHTML = "<p>Please enter a positive number.</p>";
					security = 0; 
					
					document.getElementById("securityError1").style.display = "block";
					document.getElementById("securityError1").innerHTML = "<p>Please enter a positive number for Security Monitoring Fees in the Home Maintenance section.</p>"; 
				}
				else if (security>10000000){
					document.getElementById("securityError").style.display = "block";
					document.getElementById("securityError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					security = 0; 
					
					document.getElementById("securityError1").style.display = "block";
					document.getElementById("securityError1").innerHTML = "<p>The value for Security Monitoring Fees in the Home Maintenance section must be less than 10,000,001.</p>"; 
				}
				
				else {
					document.getElementById("securityError").style.display = "none";
					document.getElementById("security").value = numberWithCommas(security.toFixed(0));
					document.getElementById("securityError1").style.display = "none";
				}
			}
		}
		calculateHousingMaintenance();
	}
	
	function updateWindowClean(){
		
		var myTemp = trimInput(document.getElementById("windowClean").value); 
		
		if (myTemp =="")
		{
			windowClean = 0; 
			document.getElementById("windowError").style.display = "none";
			document.getElementById("windowClean").value = "0";
			document.getElementById("windowError1").style.display = "none";
			
		}
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("windowError").style.display = "block";
				document.getElementById("windowError").innerHTML = "<p>Please enter a numeric value.</p>";
				windowClean = 0;
				
				document.getElementById("windowError1").style.display = "block";
				document.getElementById("windowError1").innerHTML = "<p>Please enter a numeric value for Window Cleaning in the Home Maintenance section.</p>";    
			}
			
			else {
				
				windowClean = parseFloat(myTemp);
				
				if (windowClean<0){
					document.getElementById("windowError").style.display = "block";
					document.getElementById("windowError").innerHTML = "<p>Please enter a positive number.</p>";
					windowClean = 0; 
					
					document.getElementById("windowError1").style.display = "block";
					document.getElementById("windowError1").innerHTML = "<p>Please enter a positive number for Window Cleaning in the Home Maintenance section.</p>";    
				}
				else if (windowClean>10000000){
					document.getElementById("windowError").style.display = "block";
					document.getElementById("windowError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					windowClean = 0; 
					
					document.getElementById("windowError1").style.display = "block";
					document.getElementById("windowError1").innerHTML = "<p>The value for Window Cleaning in the Home Maintenance section must be less than 10,000,001.</p>";    
				}
				else {
					document.getElementById("windowError").style.display = "none";
					document.getElementById("windowClean").value = numberWithCommas(windowClean.toFixed(0));
					document.getElementById("windowError1").style.display = "none";
				}
			}
		}
		calculateHousingMaintenance();
	}
	
	function updateGarbageCollection(){
		
		var myTemp = trimInput(document.getElementById("garbageCollection").value); 
		
		if (myTemp =="")
		{
			garbageCollection = 0; 
			document.getElementById("garbageCollectionError").style.display = "none";
			document.getElementById("garbageCollection").value = "0";
			document.getElementById("garbageCollectionError1").style.display = "none";
			
		}
		else {
				
		
			if(isNaN(myTemp)){
				document.getElementById("garbageCollectionError").style.display = "block";
				document.getElementById("garbageCollectionError").innerHTML = "<p>Please enter a numeric value.</p>";
				garbageCollection = 0; 
				
				document.getElementById("garbageCollectionError1").style.display = "block";
				document.getElementById("garbageCollectionError1").innerHTML = "<p>Please enter a numeric value for Garbage Collection in the Home Maintenance section.</p>"; 
			}
			
			else {
				
				garbageCollection = parseFloat(myTemp);
				
				if (garbageCollection<0){
					document.getElementById("garbageCollectionError").style.display = "block";
					document.getElementById("garbageCollectionError").innerHTML = "<p>Please enter a positive number.</p>";
					garbageCollection = 0; 
					
					document.getElementById("garbageCollectionError1").style.display = "block";
					document.getElementById("garbageCollectionError1").innerHTML = "<p>Please enter a positive number for Garbage Collection in the Home Maintenance section.</p>";
				}
				else if (garbageCollection>10000000){
					document.getElementById("garbageCollectionError").style.display = "block";
					document.getElementById("garbageCollectionError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					garbageCollection = 0; 
					
					document.getElementById("garbageCollectionError1").style.display = "block";
					document.getElementById("garbageCollectionError1").innerHTML = "<p>The value for Garbage Collection in the Home Maintenance section must be less than 10,000,001.</p>";
				}
				else {
					document.getElementById("garbageCollectionError").style.display = "none";
					document.getElementById("garbageCollection").value = numberWithCommas(garbageCollection.toFixed(0));
					document.getElementById("garbageCollectionError1").style.display = "none";
				}
			}
		}
		calculateHousingMaintenance();
	}
	
	
	function updateRoof(){
		
		var myTemp = trimInput(document.getElementById("roof").value);
		
		
		if (myTemp =="")
		{
			roof = 0; 
			document.getElementById("roofError").style.display = "none";
			document.getElementById("roof").value = "0";
			document.getElementById("roofError1").style.display = "none";
			
		}
		else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("roofError").style.display = "block";
				document.getElementById("roofError").innerHTML = "<p>Please enter a numeric value.</p>";
				roof =0; 
				
				document.getElementById("roofError1").style.display = "block";
				document.getElementById("roofError1").innerHTML = "<p>Please enter a numeric value for Roof in the Home Maintenance section.</p>"; 
			}
			
			else {
				
				roof = parseFloat(myTemp);
				
				if (roof<0){
					document.getElementById("roofError").style.display = "block";
					document.getElementById("roofError").innerHTML = "<p>Please enter a positive number.</p>";
					roof =0;
					
					document.getElementById("roofError1").style.display = "block";
					document.getElementById("roofError1").innerHTML = "<p>Please enter a positive number for Roof in the Home Maintenance section.</p>";  
				}
				
				else if (roof>10000000){
					document.getElementById("roofError").style.display = "block";
					document.getElementById("roofError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					roof =0;
					
					document.getElementById("roofError1").style.display = "block";
					document.getElementById("roofError1").innerHTML = "<p>The value for Roof in the Home Maintenance section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("roofError").style.display = "none";
					document.getElementById("roof").value = numberWithCommas(roof.toFixed(0));
					document.getElementById("roofError1").style.display = "none";
				}
			}
		}
			
			calculateHousingMaintenance();
			//console.log(document.getElementById("roofError").style.display == "none");
	}
	
	function updateFurnace(){
		
		var myTemp = trimInput(document.getElementById("furnace").value); 
		
		
		if (myTemp =="")
		{
			furnace = 0; 
			document.getElementById("furnaceError").style.display = "none";
			document.getElementById("furnace").value = "0";
			document.getElementById("furnaceError1").style.display = "none";
			
		}
		
		else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("furnaceError").style.display = "block";
				document.getElementById("furnaceError").innerHTML = "<p>Please enter a numeric value.</p>";
				furnace =0; 
				
				document.getElementById("furnaceError1").style.display = "block";
				document.getElementById("furnaceError1").innerHTML = "<p>Please enter a numeric value for Furnace in the Home Maintenance section.</p>"; 
			}
			
			else {
				
				furnace = parseFloat(myTemp);
			
				if (furnace<0){
					document.getElementById("furnaceError").style.display = "block";
					document.getElementById("furnaceError").innerHTML = "<p>Please enter a positive number.</p>";
					furnace =0; 
					
					document.getElementById("furnaceError1").style.display = "block";
					document.getElementById("furnaceError1").innerHTML = "<p>Please enter a positive number for Furnace in the Home Maintenance section.</p>"; 
				}
				else if (furnace>10000000){
					document.getElementById("furnaceError").style.display = "block";
					document.getElementById("furnaceError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					furnace =0; 
					
					document.getElementById("furnaceError1").style.display = "block";
					document.getElementById("furnaceError1").innerHTML = "<p>The value for Furnace in the Home Maintenance section must be less than 10,000,001.</p>"; 
				}
				else {
					document.getElementById("furnaceError").style.display = "none";
					document.getElementById("furnace").value = numberWithCommas(furnace.toFixed(0));
					document.getElementById("furnaceError1").style.display = "none";
				}
			}
		}
		
		calculateHousingMaintenance();
	}
	
	function updateAirConditioning(){
		
		var myTemp = trimInput(document.getElementById("airConditioning").value); 
		
		if (myTemp =="")
		{
			airConditioning = 0; 
			document.getElementById("acError").style.display = "none";
			document.getElementById("airConditioning").value = "0";
			document.getElementById("acError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("acError").style.display = "block";
				document.getElementById("acError").innerHTML = "<p>Please enter a numeric value.</p>";
				airConditioning = 0;
				
				document.getElementById("acError1").style.display = "block";
				document.getElementById("acError1").innerHTML = "<p>Please enter a numeric value for Air Conditioning in the Home Maintenance section.</p>";  
			}
			
			else {
				
				airConditioning = parseFloat(myTemp);
				
				if (airConditioning<0){
					document.getElementById("acError").style.display = "block";
					document.getElementById("acError").innerHTML = "<p>Please enter a positive number.</p>";
					airConditioning = 0;
					
					document.getElementById("acError1").style.display = "block";
					document.getElementById("acError1").innerHTML = "<p>Please enter a positive number for Air Conditioning in the Home Maintenance section.</p>";  
				}
				else if (airConditioning>10000000){
					document.getElementById("acError").style.display = "block";
					document.getElementById("acError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					airConditioning = 0;
					
					document.getElementById("acError1").style.display = "block";
					document.getElementById("acError1").innerHTML = "<p>The value for Air Conditioning in the Home Maintenance section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("acError").style.display = "none";
					document.getElementById("airConditioning").value = numberWithCommas(airConditioning.toFixed(0));
					document.getElementById("acError1").style.display = "none";
				}
			}
		}
		
		calculateHousingMaintenance();
	}
	
	function updateAppliances(){
		
		var myTemp = trimInput(document.getElementById("appliances").value); 
		
		if (myTemp =="")
		{
			appliances = 0; 
			document.getElementById("appliancesError").style.display = "none";
			document.getElementById("appliances").value = "0";
			document.getElementById("appliancesError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("appliancesError").style.display = "block";
				document.getElementById("appliancesError").innerHTML = "<p>Please enter a numeric value.</p>";
				appliances = 0;
				
				document.getElementById("appliancesError1").style.display = "block";
				document.getElementById("appliancesError1").innerHTML = "<p>Please enter a numeric value for Appliances in the Home Maintenance section.</p>";   
			}
			
			else {
				
				appliances = parseFloat(myTemp);
				
				if (appliances<0){
					document.getElementById("appliancesError").style.display = "block";
					document.getElementById("appliancesError").innerHTML = "<p>Please enter a positive number.</p>";
					appliances = 0; 
					
					document.getElementById("appliancesError1").style.display = "block";
					document.getElementById("appliancesError1").innerHTML = "<p>Please enter a positive number for Appliances in the Home Maintenance section.</p>"; 
				}
				else if (appliances>10000000){
					document.getElementById("appliancesError").style.display = "block";
					document.getElementById("appliancesError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					appliances = 0; 
					
					document.getElementById("appliancesError1").style.display = "block";
					document.getElementById("appliancesError1").innerHTML = "<p>The value for Appliances in the Home Maintenance section must be less than 10,000,001.</p>"; 
				}
				else {
					document.getElementById("appliancesError").style.display = "none";
					document.getElementById("appliances").value = numberWithCommas(appliances.toFixed(0));
					
					document.getElementById("appliancesError1").style.display = "none";
				}
			}
		}
		calculateHousingMaintenance();
	}
	
	function updateEmergencyRepairs(){
		
		var myTemp = trimInput(document.getElementById("emergencyRepairs").value); 
		
		if (myTemp =="")
		{
			emergencyRepairs = 0; 
			document.getElementById("emergencyError").style.display = "none";
			document.getElementById("emergencyRepairs").value = "0";
			document.getElementById("emergencyError1").style.display = "none";
			
		}
		else {
		
		
			
			if(isNaN(myTemp)){
				document.getElementById("emergencyError").style.display = "block";
				document.getElementById("emergencyError").innerHTML = "<p>Please enter a numeric value.</p>";
				emergencyRepairs = 0;
				
				document.getElementById("emergencyError1").style.display = "block";
				document.getElementById("emergencyError1").innerHTML = "<p>Please enter a numeric value for Emergency Repairs in the Home Maintenance section.</p>";    
			}
			
			else {
				
				emergencyRepairs = parseFloat(myTemp);
					
				if (emergencyRepairs<0){
					document.getElementById("emergencyError").style.display = "block";
					document.getElementById("emergencyError").innerHTML = "<p>Please enter a positive number.</p>";
					emergencyRepairs = 0;
					
					document.getElementById("emergencyError1").style.display = "block";
					document.getElementById("emergencyError1").innerHTML = "<p>Please enter a positive number for Emergency Repairs in the Home Maintenance section.</p>";     
				}
				else if (emergencyRepairs>10000000){
					document.getElementById("emergencyError").style.display = "block";
					document.getElementById("emergencyError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					emergencyRepairs = 0;
					
					document.getElementById("emergencyError1").style.display = "block";
					document.getElementById("emergencyError1").innerHTML = "<p>The value for Emergency Repairs in the Home Maintenance section must be less than 10,000,001.</p>";     
				}
				else {
					document.getElementById("emergencyError").style.display = "none";
					document.getElementById("emergencyRepairs").value = numberWithCommas(emergencyRepairs.toFixed(0));
					document.getElementById("emergencyError1").style.display = "none";
				}
			}
		}
		calculateHousingMaintenance();
	}
	
	function updateOtherMaintenance(){
		
		var myTemp = trimInput(document.getElementById("otherMaintenance").value); 
		
		if (myTemp =="")
		{
			otherMaintenance = 0; 
			document.getElementById("otherMaintenanceError").style.display = "none";
			document.getElementById("otherMaintenance").value = "0";
			document.getElementById("otherMaintenanceError1").style.display = "none";
			
		}
		else {
				
			
			
			if(isNaN(myTemp)){
				document.getElementById("otherMaintenanceError").style.display = "block";
				document.getElementById("otherMaintenanceError").innerHTML = "<p>Please enter a numeric value.</p>";
				otherMaintenance = 0; 
				
				document.getElementById("otherMaintenanceError1").style.display = "block";
				document.getElementById("otherMaintenanceError1").innerHTML = "<p>Please enter a numeric value for Other Maintenance in the Home Maintenance section.</p>"; 
			}
			
			else {
				
				otherMaintenance = parseFloat(myTemp);
				
				if (otherMaintenance<0){
					document.getElementById("otherMaintenanceError").style.display = "block";
					document.getElementById("otherMaintenanceError").innerHTML = "<p>Please enter a positive number.</p>";
					otherMaintenance = 0;
					
					document.getElementById("otherMaintenanceError1").style.display = "block";
					document.getElementById("otherMaintenanceError1").innerHTML = "<p>Please enter a positive number for Other Maintenance in the Home Maintenance section.</p>";  
				}
				else if (otherMaintenance>10000000){
					document.getElementById("otherMaintenanceError").style.display = "block";
					document.getElementById("otherMaintenanceError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					otherMaintenance = 0;
					
					document.getElementById("otherMaintenanceError1").style.display = "block";
					document.getElementById("otherMaintenanceError1").innerHTML = "<p>The value for Other Maintenance in the Home Maintenance section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("otherMaintenanceError").style.display = "none";
					document.getElementById("otherMaintenance").value = numberWithCommas(otherMaintenance.toFixed(0));
					document.getElementById("otherMaintenanceError1").style.display = "none";
				}
			}
		}
			calculateHousingMaintenance();
	}
	
	function updateOtherCosts(){
		
		var myTemp = trimInput(document.getElementById("otherCosts").value); 
		
		if (myTemp =="")
		{
			otherCosts = 0; 
			document.getElementById("otherCostsError").style.display = "none";
			document.getElementById("otherCosts").value = "0";
			document.getElementById("otherCostsError1").style.display = "none";
			
		}
		else {
			
		
		
			if(isNaN(myTemp)){
				document.getElementById("otherCostsError").style.display = "block";
				document.getElementById("otherCostsError").innerHTML = "<p>Please enter a numeric value.</p>";
				otherCosts = 0;
				
				document.getElementById("otherCostsError1").style.display = "block";
				document.getElementById("otherCostsError1").innerHTML = "<p>Please enter a numeric value for Other in the Home Maintenance section.</p>";  
			}
			
			else {
				
				otherCosts = parseFloat(myTemp);
				
				if (otherCosts<0){
					document.getElementById("otherCostsError").style.display = "block";
					document.getElementById("otherCostsError").innerHTML = "<p>Please enter a positive number.</p>";
					otherCosts = 0;
					
					document.getElementById("otherCostsError1").style.display = "block";
					document.getElementById("otherCostsError1").innerHTML = "<p>Please enter a positive number for Other in the Home Maintenance section.</p>";  
				}
				else if (otherCosts>10000000){
					document.getElementById("otherCostsError").style.display = "block";
					document.getElementById("otherCostsError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					otherCosts = 0;
					
					document.getElementById("otherCostsError1").style.display = "block";
					document.getElementById("otherCostsError1").innerHTML = "<p>The value for Other in the Home Maintenance section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("otherCostsError").style.display = "none";
					document.getElementById("otherCosts").value = numberWithCommas(otherCosts.toFixed(0));
					document.getElementById("otherCostsError1").style.display = "none";
				}
			}
		
		}
		calculateHousingMaintenance();
	}
	
	function updateFoodExpense (){
			
		var myTemp = trimInput(document.getElementById("foodExpense").value); 
		
		if (myTemp =="")
		{
			foodExpense = 0; 
			document.getElementById("foodExpensesError").style.display = "none";
			document.getElementById("foodExpense").value = "0";
			document.getElementById("foodExpensesError1").style.display = "none";
			
		}
		else {
				
			
			if(isNaN(myTemp)){
				document.getElementById("foodExpensesError").style.display = "block";
				document.getElementById("foodExpensesError").innerHTML = "<p>Please enter a numeric value.</p>";
				foodExpense = 0;
				
				document.getElementById("foodExpensesError1").style.display = "block";
				document.getElementById("foodExpensesError1").innerHTML = "<p>Please enter a numeric value for Food Expenses in the Food and Entertainment section.</p>";  
			}
			
			else {
				
				foodExpense = parseFloat(myTemp);
				
				if (foodExpense <0){
					document.getElementById("foodExpensesError").style.display = "block";
					document.getElementById("foodExpensesError").innerHTML = "<p>Please enter a positive number.</p>";
					foodExpense = 0;
					
					document.getElementById("foodExpensesError1").style.display = "block";
					document.getElementById("foodExpensesError1").innerHTML = "<p>Please enter a positive number for Food Expenses in the Food and Entertainment section.</p>";  
				}
				else if (foodExpense >10000000){
					document.getElementById("foodExpensesError").style.display = "block";
					document.getElementById("foodExpensesError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					foodExpense = 0;
					
					document.getElementById("foodExpensesError1").style.display = "block";
					document.getElementById("foodExpensesError1").innerHTML = "<p>The value for Food Expenses in the Food and Entertainment section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("foodExpensesError").style.display = "none";
					document.getElementById("foodExpense").value = numberWithCommas(foodExpense.toFixed(0));
					
					document.getElementById("foodExpensesError1").style.display = "none";
				}
			}
		
		}
		
		if (mealChoice!="default"){
			calculateFoodSaving();
		}
	}
	
	function updateMealChoice (){
		
			var mealChoiceTemp = document.getElementById("mealChoice").value; 
			
			if (mealChoiceTemp != "default"){	
				mealChoice = parseFloat(mealChoiceTemp);
				//console.log("mealChoice" + mealChoice);
				if(foodExpense!=0){
					calculateFoodSaving();
				}
			}
			else {
				
				resetMealPlan();
				calculateLivingExpense();
			}
	}
	
	function updateEntertainment (){
		
		var myTemp = trimInput(document.getElementById("entertainment").value); 
		
		if (myTemp =="")
		{
			entertainment = 0; 
			document.getElementById("entertainmentError").style.display = "none";
			document.getElementById("entertainment").value = "0";
			document.getElementById("entertainmentError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("entertainmentError").style.display = "block";
				document.getElementById("entertainmentError").innerHTML = "<p>Please enter a numeric value.</p>";
				entertainment  = 0; 
				
				document.getElementById("entertainmentError1").style.display = "block";
				document.getElementById("entertainmentError1").innerHTML = "<p>Please enter a numeric value for Entertainment Savings in the Food and Entertainment section.</p>";  
			}
			
			else {
				
				entertainment = parseFloat(myTemp);
				
				if (entertainment<0){
					document.getElementById("entertainmentError").style.display = "block";
					document.getElementById("entertainmentError").innerHTML = "<p>Please enter a positive number.</p>";
					entertainment  = 0; 
					
					document.getElementById("entertainmentError1").style.display = "block";
					document.getElementById("entertainmentError1").innerHTML = "<p>Please enter a positive number for Entertainment Savings in the Food and Entertainment section.</p>";  
				}
				
				else if (entertainment>10000000){
					document.getElementById("entertainmentError").style.display = "block";
					document.getElementById("entertainmentError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					entertainment  = 0; 
					
					document.getElementById("entertainmentError1").style.display = "block";
					document.getElementById("entertainmentError1").innerHTML = "<p>The value for Entertainment Savings in the Food and Entertainment section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("entertainmentError").style.display = "none";
					document.getElementById("entertainment").value = numberWithCommas(entertainment.toFixed(0));
					document.getElementById("entertainmentError1").style.display = "none";
				}
			}
		}
		
		calculateLivingExpense();
	}
	
	function updateLoan (){
		
		var myTemp = trimInput(document.getElementById("loan").value); 
		
		if (myTemp =="")
		{
			loan = 0; 
			document.getElementById("LeaseError").style.display = "none";
			document.getElementById("loan").value = "0";
			document.getElementById("LeaseError1").style.display = "none";
			
		}
		
		else {
			
			
			
			if(isNaN(myTemp)){
				
				document.getElementById("LeaseError").style.display = "block";
				document.getElementById("LeaseError").innerHTML = "<p>Please enter a numeric value.</p>";
				loan = 0;
				
				document.getElementById("LeaseError1").style.display = "block";
				document.getElementById("LeaseError1").innerHTML = "<p>Please enter a numeric value for Lease/Loan Payment in the Transportation section.</p>";   
			}
			
			else {
				
				loan = parseFloat(myTemp);
				
				if (loan<0){
					document.getElementById("LeaseError").style.display = "block";
					document.getElementById("LeaseError").innerHTML = "<p>Please enter a positive number.</p>";
					loan = 0;
					
					document.getElementById("LeaseError1").style.display = "block";
					document.getElementById("LeaseError1").innerHTML = "<p>Please enter a positive number for Lease/Loan Payment in the Transportation section.</p>";    
				}
				else if (loan>10000000){
					document.getElementById("LeaseError").style.display = "block";
					document.getElementById("LeaseError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					loan = 0;
					
					document.getElementById("LeaseError1").style.display = "block";
					document.getElementById("LeaseError1").innerHTML = "<p>The value for Lease/Loan Payment in the Transportation section must be less than 10,000,001.</p>";    
				}
				else {
					document.getElementById("LeaseError").style.display = "none";
					document.getElementById("loan").value = numberWithCommas(loan.toFixed(0));
					document.getElementById("LeaseError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
		
	}
	
	function updateVehicleChoice () {
		var choice = document.getElementById("vehicleChoice").value;
		if (choice=="yes"){
			document.getElementById("transportationItems").style.display = "block";
		}
		
		else {
			document.getElementById("transportationItems").style.display = "none";
			loan = 0;
			gas = 0;
			carInsurance = 0; 
			carMaintenance =0;
			license = 0; 
			parking = 0; 
			carWash = 0; 
			repairs = 0; 
			otherTransportation = 0; 
			document.getElementById("LeaseError").style.display = "none";
			document.getElementById("LeaseError1").style.display = "none";
			document.getElementById("carGasError").style.display = "none";
			document.getElementById("carGasError1").style.display = "none";
			document.getElementById("carInsuranceError").style.display = "none";
			document.getElementById("carInsuranceError1").style.display = "none";
			document.getElementById("carMaintenanceError").style.display = "none";
			document.getElementById("carMaintenanceError1").style.display = "none";
			document.getElementById("licenseError").style.display = "none";
			document.getElementById("licenseError1").style.display = "none";
			document.getElementById("parkingError").style.display = "none";
			document.getElementById("parkingError1").style.display = "none";
			document.getElementById("carWashError").style.display = "none";
			document.getElementById("carWashError1").style.display = "none";
			document.getElementById("repairsError").style.display = "none";
			document.getElementById("repairsError1").style.display = "none";
			document.getElementById("transportationError").style.display = "none";
			document.getElementById("transportationError1").style.display = "none";
			document.getElementById("loan").value = "";
			document.getElementById("gas").value = "";
			document.getElementById("carInsurance").value = "";
			document.getElementById("carMaintenance").value = "";
			document.getElementById("license").value = "";
			document.getElementById("parking").value = "";
			document.getElementById("carWash").value = "";
			document.getElementById("repairs").value = "";
			document.getElementById("otherTransportation").value = "";
	  		transportationCost = 0;
			calculateLivingExpense();
			calculateTransportationCost();
		}
	}
	
	function updateGas (){
		
		var myTemp = trimInput(document.getElementById("gas").value); 
		
		if (myTemp =="")
		{
			gas = 0; 
			document.getElementById("carGasError").style.display = "none";
			document.getElementById("gas").value = "0";
			document.getElementById("carGasError1").style.display = "none";
			
		}
		
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("carGasError").style.display = "block";
				document.getElementById("carGasError").innerHTML = "<p>Please enter a numeric value.</p>";
				gas = 0; 
				
				document.getElementById("carGasError1").style.display = "block";
				document.getElementById("carGasError1").innerHTML = "<p>Please enter a numeric value for Gas in the Transportation section.</p>";   
			}
			
			else {
				
				gas = parseFloat(myTemp);
				
				if (gas <0){
					document.getElementById("carGasError").style.display = "block";
					document.getElementById("carGasError").innerHTML = "<p>Please enter a positive number.</p>";
					gas = 0; 
					
					document.getElementById("carGasError1").style.display = "block";
					document.getElementById("carGasError1").innerHTML = "<p>Please enter a positive number for Gas in the Transportation section.</p>";   
				}
				else if (gas >10000000){
					document.getElementById("carGasError").style.display = "block";
					document.getElementById("carGasError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					gas = 0; 
					
					document.getElementById("carGasError1").style.display = "block";
					document.getElementById("carGasError1").innerHTML = "<p>The value for Gas in the Transportation section must be less than 10,000,001.</p>";   
				}
				else {
					document.getElementById("carGasError").style.display = "none";
					document.getElementById("gas").value = numberWithCommas(gas.toFixed(0));
					document.getElementById("carGasError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	function updateCarInsurance (){
		
		var myTemp = trimInput(document.getElementById("carInsurance").value); 
		
		if (myTemp =="")
		{
			carInsurance = 0; 
			document.getElementById("carInsuranceError").style.display = "none";
			document.getElementById("carInsurance").value = "0";
			document.getElementById("carInsuranceError1").style.display = "none";
			
		}
		else {
		
			
			
			if(isNaN(myTemp)){
				document.getElementById("carInsuranceError").style.display = "block";
				document.getElementById("carInsuranceError").innerHTML = "<p>Please enter a numeric value.</p>";
				carInsurance = 0;
				
				document.getElementById("carInsuranceError1").style.display = "block";
				document.getElementById("carInsuranceError1").innerHTML = "<p>Please enter a numeric value for Car Insurance in the Transportation section.</p>";    
			}
			
			else {
				
				carInsurance = parseFloat(myTemp);
				
				if (carInsurance<0){
					document.getElementById("carInsuranceError").style.display = "block";
					document.getElementById("carInsuranceError").innerHTML = "<p>Please enter a positive number.</p>";
					carInsurance = 0; 
					
					document.getElementById("carInsuranceError1").style.display = "block";
					document.getElementById("carInsuranceError1").innerHTML = "<p>Please enter a positive number for Car Insurance in the Transportation section.</p>";    
				}
				else if (carInsurance>10000000){
					document.getElementById("carInsuranceError").style.display = "block";
					document.getElementById("carInsuranceError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					carInsurance = 0; 
					
					document.getElementById("carInsuranceError1").style.display = "block";
					document.getElementById("carInsuranceError1").innerHTML = "<p>The value for Car Insurance in the Transportation section must be less than 10,000,001.</p>";    
				}
				else {
					document.getElementById("carInsuranceError").style.display = "none";
					document.getElementById("carInsurance").value = numberWithCommas(carInsurance.toFixed(0));
					document.getElementById("carInsuranceError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	function updateCarMaintenance (){
		
		var myTemp = trimInput(document.getElementById("carMaintenance").value); 
		
		if (myTemp =="")
		{
			carMaintenance = 0; 
			document.getElementById("carMaintenanceError").style.display = "none";
			document.getElementById("carMaintenance").value = "0";
			document.getElementById("carMaintenanceError1").style.display = "none";
			
		}
		
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("carMaintenanceError").style.display = "block";
				document.getElementById("carMaintenanceError").innerHTML = "<p>Please enter a numeric value.</p>";
				carMaintenance = 0; 
				
				document.getElementById("carMaintenanceError1").style.display = "block";
				document.getElementById("carMaintenanceError1").innerHTML = "<p>Please enter a numeric value for Maintenance/Inspection in the Transportation section.</p>";   
			}
			
			else {
				
				carMaintenance = parseFloat(myTemp);
				
				if (carMaintenance<0){
					document.getElementById("carMaintenanceError").style.display = "block";
					document.getElementById("carMaintenanceError").innerHTML = "<p>Please enter a positive number.</p>";
					carMaintenance = 0;
					
					document.getElementById("carMaintenanceError1").style.display = "block";
					document.getElementById("carMaintenanceError1").innerHTML = "<p>Please enter a positive number for Maintenance/Inspection in the Transportation section.</p>";  
				}
				else if (carMaintenance>10000000){
					document.getElementById("carMaintenanceError").style.display = "block";
					document.getElementById("carMaintenanceError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					carMaintenance = 0;
					
					document.getElementById("carMaintenanceError1").style.display = "block";
					document.getElementById("carMaintenanceError1").innerHTML = "<p>The value for Maintenance/Inspection in the Transportation section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("carMaintenanceError").style.display = "none";
					document.getElementById("carMaintenance").value = numberWithCommas(carMaintenance.toFixed(0));
					document.getElementById("carMaintenanceError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	function updateLicense(){
		
		var myTemp = trimInput(document.getElementById("license").value); 
		
		if (myTemp =="")
		{
			license = 0; 
			document.getElementById("licenseError").style.display = "none";
			document.getElementById("license").value = "0";
			document.getElementById("licenseError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("licenseError").style.display = "block";
				document.getElementById("licenseError").innerHTML = "<p>Please enter a numeric value.</p>";
				license = 0;
				
				document.getElementById("licenseError1").style.display = "block";
				document.getElementById("licenseError1").innerHTML = "<p>Please enter a numeric value for License/Plate Renewal in the Transportation section.</p>";   
			}
			
			else {
				
				license = parseFloat(myTemp);
				
				if (license<0){
					document.getElementById("licenseError").style.display = "block";
					document.getElementById("licenseError").innerHTML = "<p>Please enter a positive number.</p>";
					license = 0;
					
					document.getElementById("licenseError1").style.display = "block";
					document.getElementById("licenseError1").innerHTML = "<p>Please enter a positive number for License/Plate Renewal in the Transportation section.</p>";  
				}
				else if (license>10000000){
					document.getElementById("licenseError").style.display = "block";
					document.getElementById("licenseError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					license = 0;
					
					document.getElementById("licenseError1").style.display = "block";
					document.getElementById("licenseError1").innerHTML = "<p>The value for License/Plate Renewal in the Transportation section must be less than 10,000,001.</p>";  
				}
				else {
					document.getElementById("licenseError").style.display = "none";
					document.getElementById("license").value = numberWithCommas(license.toFixed(0));
					document.getElementById("licenseError1").style.display = "none";
				}
			}
		}
		calculateTransportationCost();
	}
	
	function updateParking(){
		
		var myTemp = trimInput(document.getElementById("parking").value); 
		
		
		if (myTemp =="")
		{
			parking = 0; 
			document.getElementById("parkingError").style.display = "none";
			document.getElementById("parking").value = "0";
			document.getElementById("parkingError1").style.display = "none";
			
		}
		else {
			
			
			
			if(isNaN(myTemp)){
				document.getElementById("parkingError").style.display = "block";
				document.getElementById("parkingError").innerHTML = "<p>Please enter a numeric value.</p>";
				parking = 0;
				
				document.getElementById("parkingError1").style.display = "block";
				document.getElementById("parkingError1").innerHTML = "<p>Please enter a numeric value for Parking in the Transportation section.</p>";    
			}
			
			else {
				
				parking = parseFloat(myTemp);
				
				if (parking<0){
					document.getElementById("parkingError").style.display = "block";
					document.getElementById("parkingError").innerHTML = "<p>Please enter a positive number.</p>";
					parking = 0;
					
					document.getElementById("parkingError1").style.display = "block";
					document.getElementById("parkingError1").innerHTML = "<p>Please enter a positive number for Parking in the Transportation section.</p>";     
				}
				else if (parking>10000000){
					document.getElementById("parkingError").style.display = "block";
					document.getElementById("parkingError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					parking = 0;
					
					document.getElementById("parkingError1").style.display = "block";
					document.getElementById("parkingError1").innerHTML = "<p>The value for Parking in the Transportation section must be less than 10,000,001.</p>";     
				}
				else {
					document.getElementById("parkingError").style.display = "none";
					document.getElementById("parking").value = numberWithCommas(parking.toFixed(0));
					document.getElementById("parkingError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	function updateCarWash(){
		
		var myTemp = trimInput(document.getElementById("carWash").value); 
		
		
		if (myTemp =="")
		{
			carWash = 0; 
			document.getElementById("carWashError").style.display = "none";
			document.getElementById("carWash").value = "0";
			document.getElementById("carWashError1").style.display = "none";
			
		}
		else {
		
			
			
			if(isNaN(myTemp)){
				document.getElementById("carWashError").style.display = "block";
				document.getElementById("carWashError").innerHTML = "<p>Please enter a numeric value.</p>";
				carWash = 0; 
				document.getElementById("carWashError1").style.display = "block";
				document.getElementById("carWashError1").innerHTML = "<p>Please enter a numeric value for Car Wash in the Transportation section.</p>";    
			}
			
			else {
				
				carWash = parseFloat(myTemp);
				
				if (carWash<0){
					document.getElementById("carWashError").style.display = "block";
					document.getElementById("carWashError").innerHTML = "<p>Please enter a positive number.</p>";
					carWash = 0;
					document.getElementById("carWashError1").style.display = "block";
					document.getElementById("carWashError1").innerHTML = "<p>Please enter a positive number for Car Wash in the Transportation section.</p>";   
				}
				else if (carWash>10000000){
					document.getElementById("carWashError").style.display = "block";
					document.getElementById("carWashError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					carWash = 0;
					document.getElementById("carWashError1").style.display = "block";
					document.getElementById("carWashError1").innerHTML = "<p>The value for Car Wash in the Transportation section must be less than 10,000,001.</p>";   
				}
				else {
					document.getElementById("carWashError").style.display = "none";
					document.getElementById("carWash").value = numberWithCommas(carWash.toFixed(0));
					document.getElementById("carWashError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	function updateRepairs(){
		
		var myTemp = trimInput(document.getElementById("repairs").value); 
		
		if (myTemp =="")
		{
			repairs = 0; 
			document.getElementById("repairsError").style.display = "none";
			document.getElementById("repairs").value = "0";
			document.getElementById("repairsError1").style.display = "none";
			
		}
		
		else {
			
			
			if(isNaN(myTemp)){
				document.getElementById("repairsError").style.display = "block";
				document.getElementById("repairsError").innerHTML = "<p>Please enter a numeric value.</p>";
				repairs = 0;
				
				document.getElementById("repairsError1").style.display = "block";
				document.getElementById("repairsError1").innerHTML = "<p>Please enter a numeric value for Repairs in the Transportation section.</p>";    
			}
			
			else {
				
				repairs = parseFloat(myTemp);
				
				if (repairs<0){
					document.getElementById("repairsError").style.display = "block";
					document.getElementById("repairsError").innerHTML = "<p>Please enter a positive number.</p>";
					repairs = 0;
					
					document.getElementById("repairsError1").style.display = "block";
					document.getElementById("repairsError1").innerHTML = "<p>Please enter a positive number for Repairs in the Transportation section.</p>"; 
				}
				else if (repairs>10000000){
					document.getElementById("repairsError").style.display = "block";
					document.getElementById("repairsError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					repairs = 0;
					
					document.getElementById("repairsError1").style.display = "block";
					document.getElementById("repairsError1").innerHTML = "<p>The value for Repairs in the Transportation section must be less than 10,000,001.</p>"; 
				}
				else {
					document.getElementById("repairsError").style.display = "none";
					document.getElementById("repairs").value = numberWithCommas(repairs.toFixed(0));
					document.getElementById("repairsError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	function updateOtherTransportation(){
		
		var myTemp = trimInput(document.getElementById("otherTransportation").value); 
		
		if (myTemp =="")
		{
			otherTransportation = 0; 
			document.getElementById("transportationError").style.display = "none";
			document.getElementById("otherTransportation").value = "0";
			document.getElementById("transportationError1").style.display = "none";
			
		}
		else {
			
			if(isNaN(myTemp)){
				document.getElementById("transportationError").style.display = "block";
				document.getElementById("transportationError").innerHTML = "<p>Please enter a numeric value.</p>";
				otherTransportation = 0; 
				
				document.getElementById("transportationError1").style.display = "block";
				document.getElementById("transportationError1").innerHTML = "<p>Please enter a numeric value for Other Types of Transportation in the Transportation section.</p>";    
			}
			else {
				
				otherTransportation = parseFloat(myTemp);
				
				if (otherTransportation<0){
					document.getElementById("transportationError").style.display = "block";
					document.getElementById("transportationError").innerHTML = "<p>Please enter a positive number.</p>";
					otherTransportation = 0; 
					
					document.getElementById("transportationError1").style.display = "block";
					document.getElementById("transportationError1").innerHTML = "<p>Please enter a positive number for Other Types of Transportation in the Transportation section.</p>";
				}
				else if (otherTransportation>10000000){
					document.getElementById("transportationError").style.display = "block";
					document.getElementById("transportationError").innerHTML = "<p>The maximum value is 10,000,000.</p>";
					otherTransportation = 0; 
					
					document.getElementById("transportationError1").style.display = "block";
					document.getElementById("transportationError1").innerHTML = "<p>The value for Other Types of Transportation in the Transportation section must be less than 10,000,001.</p>";
				}
				else {
					document.getElementById("transportationError").style.display = "none";
					document.getElementById("otherTransportation").value = numberWithCommas(otherTransportation.toFixed(0));
					document.getElementById("transportationError1").style.display = "none";
				}
			}
		}
		
		calculateTransportationCost();
	}
	
	
	
	function gotoHome (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "block"; 
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none"; 
		highlight("btn-homeEquity");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-results");
		$('html, body').animate({scrollTop:0}, 'slow');  
	}
	
	function preEquity (){
		window.location.hash = '#titleOnline';
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none"; 
		document.getElementById("income").style.display = "block"; 
		highlight("btn-income");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-results");
		$('html, body').animate({scrollTop:0}, 'slow');  
	}
	
	function nextEquity (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "block";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none"; 
		highlight("btn-expense");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-results");
		$('html, body').animate({scrollTop:0}, 'slow');   
	}
	
	function preHousingExpenses (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "block";
		document.getElementById("housing_expenses").style.display = "none"; 
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none";
		highlight("btn-homeEquity");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results");
		$('html, body').animate({scrollTop:0}, 'slow');   
		 
	}
	
	function nextHousingExpenses (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "block";
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none"; 
		turnOffHiglight("btn-homeEquity");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		highlight ("btn-maintainence");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results");
		$('html, body').animate({scrollTop:0}, 'slow');   
	}
	
	function preHomeMaintenance (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "block";
		document.getElementById("home_maintenance").style.display = "none";
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none";
		highlight("btn-expense");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-results");  
		$('html, body').animate({scrollTop:0}, 'slow');  
	}
	
	function nextHomeMaintenance (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("food_expenses").style.display = "block";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none";
		highlight("btn-food");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results"); 
		$('html, body').animate({scrollTop:0}, 'slow');  
	}
	
	function preFood (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";	
		document.getElementById("food_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "block";
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("result").style.display = "none"; 
		highlight("btn-maintainence");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results");  
		$('html, body').animate({scrollTop:0}, 'slow'); 
	}
	
	function nextFood (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";
		document.getElementById("transportation").style.display = "block"; 
		document.getElementById("result").style.display = "none";
		highlight("btn-transportation");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results"); 
		$('html, body').animate({scrollTop:0}, 'slow');
	}
	
	function preTransportation (){
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "block";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("result").style.display = "none";
		highlight("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results");
		$('html, body').animate({scrollTop:0}, 'slow');   
	}
	
	function nextTransportation (){
		document.getElementById("result").style.display = "block";
		document.getElementById("transportation").style.display = "none"; 
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none"; 
		document.getElementById("transportation").style.display = "none"; 
		highlight("btn-results");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-transportation");
		$('html, body').animate({scrollTop:0}, 'slow'); 
	}
	
	function preResult (){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "block";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";  
		document.getElementById("result").style.display = "none";  
		highlight("btn-transportation");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results"); 
		$('html, body').animate({scrollTop:0}, 'slow');
	}
	
	function gotoResult(){
		document.getElementById("result").style.display = "block";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";
		highlight("btn-results");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		//displayError ();
		//document.getElementById("btn-results").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-results").style.color = "white";
		
	}
	
	function gotoTransportation(){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "block";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";  
		document.getElementById("result").style.display = "none"; 
		highlight("btn-transportation");
		turnOffHiglight ("btn-results");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		//document.getElementById("btn-transportation").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-transportation").style.color = "white"; 
	}
	
	function gotoFood(){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "block";  
		document.getElementById("result").style.display = "none";
		highlight("btn-food");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-results");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		//document.getElementById("btn-food").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-food").style.color = "white";  
	}
	
	function gotoMaintainence(){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "block"; 
		document.getElementById("food_expenses").style.display = "none";  
		document.getElementById("result").style.display = "none";  
		highlight("btn-maintainence");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-results");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		//document.getElementById("btn-maintainence").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-maintainence").style.color = "white";
	}
	
	function gotoHousingExpenses(){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "block";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";  
		document.getElementById("result").style.display = "none";
		highlight("btn-expense");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-results");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-income");
		//document.getElementById("btn-expense").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-expense").style.color = "white";  
	}
	
	function gotoHomeEquity(){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("income").style.display = "none";
		document.getElementById("equity").style.display = "block";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";  
		document.getElementById("result").style.display = "none";
		highlight("btn-homeEquity");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-results");
		turnOffHiglight ("btn-income");
		//document.getElementById("btn-homeEquity").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-homeEquity").style.color = "white";    
	}
	
	function gotoIncome(){
		document.getElementById("result").style.display = "none";
		document.getElementById("transportation").style.display = "none";
		document.getElementById("income").style.display = "block";
		document.getElementById("equity").style.display = "none";
		document.getElementById("housing_expenses").style.display = "none";
		document.getElementById("home_maintenance").style.display = "none"; 
		document.getElementById("food_expenses").style.display = "none";  
		document.getElementById("result").style.display = "none";
		highlight("btn-income");
		turnOffHiglight ("btn-transportation");
		turnOffHiglight ("btn-food");
		turnOffHiglight ("btn-maintainence");
		turnOffHiglight ("btn-expense");
		turnOffHiglight ("btn-homeEquity");
		turnOffHiglight ("btn-results");  
		//document.getElementById("btn-income").style.backgroundColor = "#B3C2D1";
		//document.getElementById("btn-income").style.color = "white";    
	}
	
	function highlight (x) {
		document.getElementById(x).style.backgroundColor = "#B3C2D1";
		document.getElementById(x).style.color = "white";
	}
	
	function turnOffHiglight (y) {
		document.getElementById(y).style.backgroundColor = "white";
		document.getElementById(y).style.color = "black"; 
	}
	
	function infoPostRetirement(){
		document.getElementById("information").style.display = "block";
	}
	
	function closeBox(){
		document.getElementById("information").style.display = "none";
	}
	
	function infoTenant(){
		document.getElementById("tenantInsurancemyTemp").style.display = "block";
	}
	
	function closeBoxTenant(){
		document.getElementById("tenantInsurancemyTemp").style.display = "none";
	}
	
	function infoUtilities(){
		document.getElementById("utilities").style.display = "block";
	}
	
	function closeUtilities(){
		document.getElementById("utilities").style.display = "none";
	}
	
	function infoFood(){
		document.getElementById("food").style.display = "block";
	}
	
	function closeFood(){
		document.getElementById("food").style.display = "none";
	}

	function infoActivities(){
		document.getElementById("activities").style.display = "block";
	}
	
	function closeActivities(){
		document.getElementById("activities").style.display = "none";
	}
	
	function infoEntertainment(){
		document.getElementById("entertainmentList").style.display = "block";
	}
	
	function closeEntertainment(){
		document.getElementById("entertainmentList").style.display = "none";
	}
	
	function reCalculate() {
		document.getElementById("refreshPage").style.display = "block";
    }
	
	function restartTool (){
		location.reload();
		document.getElementById("refreshPage").style.display = "none";
	}
	
	function turnOffRefresh (){
		document.getElementById("refreshPage").style.display = "none";
	}
	
	function printResults() {
		
		var pensionRe = pensionTemp;
		var cashProceedsRe = cashProceeds;
		var totalIncomeRe = monthlyIncome;
		var printBtn = "<br/><div class='btn btn-info' onclick='window.print();'>Print</div>"
		HTMLstring += "<h1>Print a copy for your own record</h1><br/>";
		HTMLstring += "<b>Pension:</b>" + pensionRe + "<br/>";
		HTMLstring +="<b>Cash Proceeds</b>: "+ cashProceedsRe + "<br/>";
		HTMLstring += "<b>Monthly Income:</b> " + totalIncomeRe + "<br/>";
		HTMLstring += printBtn;
		
		newwindow=window.open('','','left=10,top=10,width=600,height=800');
		newdocument=newwindow.document;
		newdocument.write(HTMLstring);
		newdocument.close();  
	}
	
	/*function printResult() {
		document.getElementById("ba-CSS").setAttribute('href', 'ba-print.css');
		//window.print();
		document.getElementById("home_sell").disabled = true;
		document.getElementById("rate").disabled = true;
		document.getElementById("mealChoice").disabled = true;
		document.getElementById("vehicleChoice").disabled = true;
	}*/
	
	function printResult() {
		
		
		var browserName = navigator.appName; 
		
		if (browserName == "Microsoft Internet Explorer"){
			
			var content = document.getElementById("container"); 
			var disclaimer  = document.getElementById("disclaimer");
			
			newWin= window.open();
			
			newWin.document.write(content.innerHTML);
			newWin.document.write(disclaimer.innerHTML);
			newWin.document.getElementById("home_sell").disabled = true;
			newWin.document.getElementById("rate").disabled = true;
			newWin.document.getElementById("mealChoice").disabled = true;
			newWin.document.getElementById("vehicleChoice").disabled = true;
		
		}
		else {
			window.print();
		}
	}
	
	
	function cancelPrint() {
		document.getElementById("ba-CSS").setAttribute('href', 'ba-default-fr.css');
		document.getElementById("home_sell").disabled = false;
		document.getElementById("rate").disabled = false;
		document.getElementById("mealChoice").disabled = false;
		document.getElementById("vehicleChoice").disabled = false;
	}
	
	
	function onPagePrint() {
		window.print();
		document.getElementById("ba-CSS").setAttribute('href', 'ba-default.css');
		document.getElementById("home_sell").disabled = false;
		document.getElementById("rate").disabled = false;
		document.getElementById("mealChoice").disabled = false;
		document.getElementById("vehicleChoice").disabled = false;
		
	}