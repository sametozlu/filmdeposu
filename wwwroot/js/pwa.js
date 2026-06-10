(function () {
  if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/sw.js').catch(function () {});
  }

  var installBtn = document.getElementById('pwaInstallBtn');
  var deferredPrompt = null;

  window.addEventListener('beforeinstallprompt', function (e) {
    e.preventDefault();
    deferredPrompt = e;
    if (installBtn) installBtn.hidden = false;
  });

  if (installBtn) {
    installBtn.addEventListener('click', function () {
      if (!deferredPrompt) return;
      deferredPrompt.prompt();
      deferredPrompt.userChoice.finally(function () {
        installBtn.hidden = true;
        deferredPrompt = null;
      });
    });
  }

  document.querySelectorAll('.skeleton-img').forEach(function (img) {
    img.classList.add('is-loading');
    function done() { img.classList.remove('is-loading'); }
    if (img.complete) done();
    else { img.addEventListener('load', done); img.addEventListener('error', done); }
  });
})();
