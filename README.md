# AYOKA
Fall detection system for runners and cyclists based on GPS-inactivty

This is a prototype and is part of a Bachelor Thesis

### ON SECRECY

Your location is tracked and there are permissons to call and send an receive SMS.
READ PHONE STATE is not used ATM but might be in future versions.

This app uses  some of these permissions and otherrs are for future possible versions:
	uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"
	uses-permission android:name="android.permission.INTERNET"
	uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION"
	uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"
	uses-permission android:name="android.permission.ACCESS_MOCK_LOCATION"
	uses-permission android:name="android.permission.ACCESS_LOCATION_EXTRA_COMMANDS"
	uses-permission android:name="android.permission.BATTERY_STATS"
	uses-permission android:name="android.permission.ACCESS_CHECKIN_PROPERTIES"
	uses-permission android:name="android.permission.FOREGROUND_SERVICE"
	uses-permission android:name="android.permission.INSTANT_APP_FOREGROUND_SERVICE"
	uses-permission android:name="android.permission.ANSWER_PHONE_CALLS"
	uses-permission android:name="android.permission.CALL_PHONE"
	uses-permission android:name="android.permission.SEND_SMS"
	uses-permission android:name="android.permission.RECEIVE_SMS"
	uses-permission android:name="android.permission.READ_PHONE_STATE"

*****
Deactivated and commented out:

Handling messages sent from application to iotHub:
The class DeviceToCloud 
method SendMessages() in Monitor.

Call function needs more work as well as to play Text-to-speech in a call.
*****


# Manual

### 1. 
Launch app
### 2. 
A notification text on required permissions appears  - click OK.
### 3. 
Allow permissions for accessing location, sending sms and  to make phone calls.
### 4. 
Wait for the app to locate you (You are located when the enabled blue button has the text ACTIVATE).
<p align="center">
	
<img src="https://lh4.googleusercontent.com/5XeuhEOc01RAGDDbD5ZRbsxpqYNfoyiFjd8OOdVlll1I_yilQ69TR8UES9KeMpAP3L2mmm8nMxPovvOOld2tnuTQs8h6Wduhc8waGC5o"/>
</p>







### 5. 
Select the Settings page tab.
### 6. 
Add contacts by pressing the plus icon in the upper right corner.
<p align="center">
<img src="https://lh5.googleusercontent.com/Ckdwal9ZvCn31mHf0V2SdJ3eqYzWBO3BW4G8Q6LnQKg72ZEHHtytbutQY9AlLuIsmDLQE44r7eGceR9e8GoP7i-8TF-OjZIzR25zrpcd"/> <img src="https://lh4.googleusercontent.com/q5VpVoDinJ3Q7qSER_sdydT9DlSSnzdtkfQ0RafG_IFi3KQjIKjSilut2XVHqxZ3_MzszMn9lWY33-b5y78m_Awn5PsED2qcTc_18w7B"/>
	</p>
		


### 7. 
Select length of user dialogue. This is time that sets the duration  you have to confirm all is OK if the app has detected an inactivity (10 - 60 seconds) before the app alarms.
### 8. 
Select the time interval of an inactivity. This is the interval (1-5 min) the app uses to check your current location within .
### 9. 
Edit your text message that will be sent to your contacts together with information about your location, the time, date and a link with the exact location to google maps.
### 10. 
Go back to the Home page.
### 11. 
Press ACTIVATE  - this is when latest edited settings are used, settings can´t be changed during a session.
### 12. 
You will now see the remaining time to next check on the blue button which is also used to deactivate the detecting session. The initial location (coordinates )is displayed on the same page and will be filled with the next. 

<p align="center">
<img src="https://lh5.googleusercontent.com/5qGWC33WYLSQE7L4lm5r-zPRyPufGKNr8Xiqrdf1xBDopUwTdtAT9s4loARvT6ELV8cnIRA4I-cNjoMnjRS0QqKr0CeL5Wg0fsx9JVtm"/>
</p>

### 13. 
If an Inactivity is detected - an alarm dialogue starts and the app will ask you if you are OK. If you are ok press OK- the session continues.

<p align="center">
<img src="https://lh5.googleusercontent.com/1k6odSaYUnDrlI4NExT_xOVPCnwwBQ95stv7FF-wlvy_ejqUdIMU1maQtgwMN2n-uaRFY8UzdJd9Cbf7e-j19SE8zjBeEkV9CG8Ghauu"/> <img src="https://lh4.googleusercontent.com/6um1qrtAuR2udF_ftPoDRIBm7tOnk6ABnEHGfyd-LomWliwrS6ijemkYOBgWpq3QHHiwy5Ka0-sNm4YoxQwTTstq6fbq98LEvUtBvvXC"/>
</p>




### 14. 
If no interaction is perceived from the user, the system stops the current session, sends an sms with the edited text and the last known location to the numbers in your contact list. The Phone call functionality i currently deactivated.

<p align="center">
<img src="https://lh4.googleusercontent.com/vSDzUXJmOtS40ar8Gy0US8_jEgN5PwmUvis9Rszpfmjo2WGwxY1_IN8L_7uUWoNXQNdWg0lXfQfKTB_uL32FZSOVEkfim6hYKWEGgySU"/>
	</p>




### 15. 
Geo locations are currently retrieved at all times from the app. Only checked geo locations are saved locally and all geolocations in one session are sent at once via IoT Hub and Stream Analytics and stored in an Azure SQL database when a session is deactivated or on alarm.




