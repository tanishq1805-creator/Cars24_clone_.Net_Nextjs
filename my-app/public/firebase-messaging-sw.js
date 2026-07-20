importScripts("https://www.gstatic.com/firebasejs/10.13.2/firebase-app-compat.js");
importScripts("https://www.gstatic.com/firebasejs/10.13.2/firebase-messaging-compat.js");

firebase.initializeApp({
  apiKey: "AIzaSyB3dtIuB3KGBJR0NHFheq2kIm7-1S6V-NU",
  authDomain:  "cars24-clone-d0698.firebaseapp.com",
  projectId: "cars24-clone-d0698",
  storageBucket: "cars24-clone-d0698.firebasestorage.app",
  messagingSenderId: "845156259747",
  appId: "1:845156259747:web:5f055783f10fa4ae142c16",
});

const messaging = firebase.messaging();

messaging.onBackgroundMessage(function (payload) {
  self.registration.showNotification(payload.notification.title, {
    body: payload.notification.body,
    icon: "/logo.png",
  });
});