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


# Manual

### 1. 
Launch app
### 2. 
A notification text on required permissions appears  - click OK.
### 3. 
Allow permissions for accessing location, sending sms and  to make phone calls.
### 4. 
Wait for the app to locate you (You are located when the enabled blue button has the text ACTIVATE).

![](https://lh4.googleusercontent.com/GHzVihFr-sZUkiJ5LZtcRTc-vFn9ds52PUM-dQfIwoPXWEyw8k-jzgTKh-GXJKOP-xcEAKNtg1UA6Ix0MK8oskXY4OXlRtybX2b_gDbH)









### 5. 
Select the Settings page tab.
### 6. 
Add contacts by pressing the plus icon in the upper right corner.

![](https://lh6.googleusercontent.com/cnlNDLn4YsxbL1aNo_vOnmQcetg4iuHgXZVCjNgXJvf1M-WEOFvWqu451rlhnfviPYITcJQW1o3T2MUUw8Rbzr2GRfZkGEcttx1qxJX8)   ![](https://lh4.googleusercontent.com/jHQRTuerGcqYAZflQvcG_jl9Wb-_0GEgX6psLy7BBATdpWsmnXKR3etsZZAC4y912oioDm8L9TSW9G-m3VAAYPAoVIKU8bNuWOgZ3kkE)
		


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

![](https://lh6.googleusercontent.com/qL4BUrHVscL5YCtBkgTS4_VAhUNaRvCWQtAiwqTGhmXsvb391xNBgMb3ECV_53Bh0mVgmWqoxqrB0wjXrww5QRcoRFkFdMjN6s_FN3Jj)

### 13. 
If an Inactivity is detected - an alarm dialogue starts and the app will ask you if you are OK. If you are ok press OK- the session continues.

![](https://lh5.googleusercontent.com/Y-wc2Nrf_B5i-UScu85rh-Mi21RBmZWNzp-Vojpf82fqeLF0MzuhB78iM5ma9tvpoqVRJJ4w8atBKjvzsrT3YJ5A7ZbBfnY31MvzXYQf)   ![](https://lh4.googleusercontent.com/T4Dbde5sfvafHxhc9nEKoQfmGalUwnVraRbmD9Q206yCPy7FNnpXbVRxwc0GMwVpNwFwG3qieEbNOgImvp6WVK1GA0tGv5ba62cQO66v)

### 14. 
If no interaction is perceived from the user, the system stops the current session, sends an sms with the edited text and the last known location to the numbers in your contact list.

![](https://lh5.googleusercontent.com/s_hdBbQm1iFP1WVR6JusLQGDR3o8acmgKu3x6VgN_DIMmjI9ZPM2coGFewBAJBxNW2OEdLs5TMt7AxOy-PW4wsSCaUPkHetW-vOlRXdR)


### 15. 
Geo locations are currently retrieved at all times from the app. Only checked geo locations are saved locally and all geolocations in one session are sent at once via IoT Hub and Stream Analytics and stored in an Azure SQL database when a session is deactivated or on alarm.

