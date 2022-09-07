// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', () =>
{
    setInterval(doStuff, 5000);
});

function doStuff() {
    console.log("hello!");
}

self.addEventListener('install', () =>
{
    console.log("Service worker installed.");
});

self.addEventListener('activate', () =>
{
    console.log("Service worker activated.");
});
