# AYOKA
Fall detection system for runners and cyclists based on GPS-inactivty

This is a prototype and is part of a Bachelor Thesis

*****
Deactivated and commented out:

Handling messages sent from application to iotHub:
The class DeviceToCloud 
method SendMessages() in Monitor.

Call function needs more work as well as to play Text-to-speech in a call.
*****


Manual:

1.
Launch app
2.
A notification text on required permissions appears  - click OK.
3.
Allow permissions for accessing location, sending sms and  to make phone calls.
4.
Wait for the app to locate you (You are located when the enabled blue button has the text ACTIVATE).
5.
Select the Settings page tab.
6.
Add contacts by pressing the plus icon in the upper right corner.
7.
Select length of user dialogue. This is time that sets the duration  you have to confirm all is OK if the app has detected an inactivity
(10 - 60 seconds) before the app alarms.
8.
Select the time interval of an inactivity. This is the interval (1-5 min) the app uses to check your current location within .
9.
Edit your text message that will be sent to your contacts together with information about your location, the time, date and a link with the exact location to google maps.
10.
Go back to the Home page.
11.
Press ACTIVATE  - this is when latest edited settings are used, settings can´t be changed during a session.
12.
You will now see the remaining time to next check on the blue button which is also used to deactivate the detecting session. The initial location (coordinates )is displayed on the same page and will be filled with the next. 
13.
If an Inactivity is detected - an alarm dialogue starts and the app will ask you if you are OK. If you are ok press OK- the session continues.
14.
If no interaction is perceived from the user, the system stops the current session, sends an sms with the edited text and the last known location to the numbers in your contact list.
15.
Geo locations are currently retrieved at all times from the app. Only checked geo locations are saved locally and all geolocations in one session are sent at once via IoT Hub and Stream Analytics and stored in an Azure SQL database when a session is deactivated or on alarm.


