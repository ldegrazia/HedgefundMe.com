using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using paycircuit.com.google.iap;
namespace HedgefundMe.com.Services
{
    public class GoogleWalletService
    {
        public const string MY_SELLER_ID = "13151487272455685111";
        public const string MY_SELLER_SECRET = "S-0lgfqBtqZ_nITMVpqhHA";
        public const string MY_POSTBACK_TYP = "google/payments/inapp/item/v1/postback/buy"; //Value is CaSE-sEnsitive (strict verification of JWT).  
        public const string THE_ISSUER = "Google"; //Value is CaSE-sEnsitive (strict verification of JWT).
        public const bool STRICT_VERIFICATION = true; // True: strict verification of JWT, False: header/signature verification 

        public string CreateSubscriptionJwt(string emailaddress)
        {
            try
            {
                JWTHeaderObject HeaderObj = new JWTHeaderObject(JWTHeaderObject.JWTHash.HS256, "1", "JWT");

                InAppItemSubscriptionRequestObject request = new InAppItemSubscriptionRequestObject()
                {
                    name = "HedgeFundMe.com Monthly Access",
                    description = "Full access to HedgeFundMe.com every month!",
                    initialPayment = new InAppSubscriptionInitialPaymentObject()
                    {
                        price = "0.0",
                        currencyCode = RequestObject.SupportedCurrencies.USD.ToString(),
                        paymentType = "prorated"
                    },
                    sellerData = emailaddress,
                    recurrence = new InAppSubscriptionRecurrenceObject()
                    {
                        frequency = "monthly",
                        price = "29.00",
                        currencyCode = RequestObject.SupportedCurrencies.USD.ToString(),
                        recurrenceStartDate = DateTime.Now.AddMonths(1),  //start recurrence a month from today
                        numRecurrences = "24"
                    }
                };
                InAppItemObject ClaimObj = new InAppItemObject(request) { iss = MY_SELLER_ID };
                return JWTHelpers.buildJWT(HeaderObj, ClaimObj, MY_SELLER_SECRET);
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Parse and verify the JWT POST from Google (signature verification)
        /// </summary>
        /// <param name="jstring">JWT value</param> 
        public List<string> ParseJWT(string jstring)
        {
            string _jwtHeader = string.Empty;
            string _jwtClaim = string.Empty;
            Logger.WriteLine(MessageType.Information, "Received from google:" + jstring);
            //verify the JWT
            if (JWTHelpers.verifyJWT(jstring, MY_SELLER_SECRET, ref _jwtHeader, ref _jwtClaim))
            { 
                    //if the JWT is successfully verified (true), proceed to deserialize
                    JWTHeaderObject HeaderObj = JSONHelpers.dataContractJSONToObj(_jwtHeader, new JWTHeaderObject()) as JWTHeaderObject;
                    InAppItemObject ClaimObj = JSONHelpers.dataContractJSONToObj(_jwtClaim, new InAppItemObject()) as InAppItemObject; 
                    //You can do whatever you need to do with the data           
                    return ParsePayload(ClaimObj, HeaderObj); 
            }
            else
            {
                throw new Exception("parseJWT Verification failure");
                
            }
        }
        /// <summary>
        /// Sample: Handle both types of POSTBACKS from Google: Success and Failure. Must alos distinguish between single and subscription item success POSTBACKs
        /// If success returns user email and then order id
        /// </summary>
        /// <param name="ClaimObj">JWT Payload</param>
        /// <param name="HeaderObj">JWT Header</param>
        public List<string> ParsePayload(InAppItemObject ClaimObj, JWTHeaderObject HeaderObj)
        {
            //header JWTHeaderObject
            string useremail = "foo";
            string orderid = "-1";
            string foo = string.Format("JWT Headers{0}JWT Algo: {1}{0}JWT kid: {2}{0}JWT typ: {3}{0}{0}", Environment.NewLine, HeaderObj.alg, HeaderObj.kid, HeaderObj.typ);
            string bar = string.Format("JWT Payload{0}JWT aud: {1}{0}JWT iss: {2}{0}JWT orderid: {3}{0}JWT sellerdata: {4}{0}JWT iat: {5}{0}JWT itemName: {6}{0}JWT itemPrice: {7:c}{0}JWT Item Description: {8}{0}JWT exp: {9}{0}JWT typ: {10}{0}{0}",
                                              Environment.NewLine,
                                              ClaimObj.aud,
                                              ClaimObj.iss,
                                              ClaimObj.response.orderId,
                                              ClaimObj.isSuccessPostback ? ClaimObj.request.sellerData : "Failure Postback does not have request.sellerData",
                                              ClaimObj.iat,
                                              ClaimObj.isSuccessPostback ? ClaimObj.request.name : "Failure Postback does not have request.name",
                                              ClaimObj.isSuccessPostback && ClaimObj.isSingleItem ? ClaimObj.request.price :
                                              ClaimObj.isSuccessPostback && ClaimObj.isSubscriptionItem ? ClaimObj.request.initialPayment.price : "Failure Postback does not have request.price",
                                              ClaimObj.isSuccessPostback ? ClaimObj.request.description : "Failure Postback does not have request.description",
                                              ClaimObj.exp,
                                              ClaimObj.typ
                                              );
            Logger.WriteLine(MessageType.Information, foo);
            Logger.WriteLine(MessageType.Information, bar);
            if (ClaimObj.isFailurePostback)
            {
                var error = String.Format("Failure for {0} Status Code: {1}",ClaimObj.response.orderId, ClaimObj.response.statusCode);
                Logger.WriteLine(MessageType.Error, error);
                throw new Exception(String.Format("Failure Status Code: {0}", ClaimObj.response.statusCode));
            }
            else
            {
                //we need the user id and order
                useremail = ClaimObj.request.sellerData;
                orderid = ClaimObj.response.orderId;
            }
            return new List<string> {useremail,orderid}; 
        }
    }
}