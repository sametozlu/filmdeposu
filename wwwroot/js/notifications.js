(function () {
  'use strict';

  if (typeof signalR === 'undefined') return;

  var myUserId = document.body.dataset.userId || '';

  var connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/notifications')
    .withAutomaticReconnect()
    .build();

  connection.on('reviewPosted', function (data) {
    if (data.userId && data.userId === myUserId) return;
    var stars = '★'.repeat(data.rating) + '☆'.repeat(5 - data.rating);
    showToast(
      '💬 ' + data.author,
      '"' + data.seriesTitle + '" — ' + stars,
      '/Series/Detail/' + data.seriesId + '#reviews'
    );
  });

  connection.start().catch(function () { /* sessizce geç */ });

  function showToast(title, body, url) {
    var wrap = document.getElementById('toastWrap');
    if (!wrap) {
      wrap = document.createElement('div');
      wrap.id = 'toastWrap';
      wrap.className = 'toast-wrap';
      document.body.appendChild(wrap);
    }

    var toast = document.createElement('a');
    toast.className = 'toast';
    toast.href = url;
    toast.innerHTML = '<strong></strong><span></span>';
    toast.querySelector('strong').textContent = title;
    toast.querySelector('span').textContent = body;
    wrap.appendChild(toast);

    setTimeout(function () { toast.classList.add('show'); }, 30);
    setTimeout(function () {
      toast.classList.remove('show');
      setTimeout(function () { toast.remove(); }, 400);
    }, 6000);
  }
})();
