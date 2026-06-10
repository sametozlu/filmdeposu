(function () {
  'use strict';

  var typeIcons = { series: '🎬', movie: '🎞️', actor: '🎭' };
  var overlay, input, list, items = [], activeIndex = -1, debounceTimer;

  function build() {
    overlay = document.createElement('div');
    overlay.className = 'cp-overlay';
    overlay.hidden = true;
    overlay.innerHTML =
      '<div class="cp-box" role="dialog" aria-label="Search">' +
      '  <div class="cp-input-wrap">' +
      '    <span class="cp-icon">🔍</span>' +
      '    <input type="text" class="cp-input" placeholder="' + (window.cpPlaceholder || 'Seri, film veya oyuncu ara...') + '" autocomplete="off" />' +
      '    <kbd class="cp-kbd">ESC</kbd>' +
      '  </div>' +
      '  <ul class="cp-list"></ul>' +
      '  <div class="cp-footer"><span>↑↓ ' + (window.cpNavHint || 'gezin') + '</span><span>↵ ' + (window.cpOpenHint || 'aç') + '</span></div>' +
      '</div>';
    document.body.appendChild(overlay);

    input = overlay.querySelector('.cp-input');
    list = overlay.querySelector('.cp-list');

    overlay.addEventListener('mousedown', function (e) {
      if (e.target === overlay) close();
    });

    input.addEventListener('input', function () {
      clearTimeout(debounceTimer);
      debounceTimer = setTimeout(search, 200);
    });

    input.addEventListener('keydown', function (e) {
      if (e.key === 'ArrowDown') { e.preventDefault(); move(1); }
      else if (e.key === 'ArrowUp') { e.preventDefault(); move(-1); }
      else if (e.key === 'Enter') { e.preventDefault(); openActive(); }
    });
  }

  function open() {
    if (!overlay) build();
    overlay.hidden = false;
    document.body.style.overflow = 'hidden';
    input.value = '';
    render([]);
    setTimeout(function () { input.focus(); }, 30);
  }

  function close() {
    if (!overlay) return;
    overlay.hidden = true;
    document.body.style.overflow = '';
  }

  function search() {
    var q = input.value.trim();
    if (q.length < 2) { render([]); return; }
    fetch('/api/v1/search?q=' + encodeURIComponent(q))
      .then(function (r) { return r.json(); })
      .then(render)
      .catch(function () { render([]); });
  }

  function render(data) {
    items = data || [];
    activeIndex = items.length ? 0 : -1;
    if (!items.length) {
      list.innerHTML = input && input.value.trim().length >= 2
        ? '<li class="cp-empty">' + (window.cpNoResults || 'Sonuç bulunamadı') + '</li>'
        : '';
      return;
    }
    list.innerHTML = items.map(function (item, i) {
      return '<li class="cp-item' + (i === activeIndex ? ' active' : '') + '" data-i="' + i + '">' +
        '<img src="' + item.image + '" alt="" loading="lazy" onerror="this.style.visibility=\'hidden\'" />' +
        '<div class="cp-item-text"><strong>' + escapeHtml(item.title) + '</strong><span>' + escapeHtml(item.subtitle || '') + '</span></div>' +
        '<span class="cp-type">' + (typeIcons[item.type] || '') + '</span>' +
        '</li>';
    }).join('');

    list.querySelectorAll('.cp-item').forEach(function (el) {
      el.addEventListener('click', function () {
        activeIndex = parseInt(el.dataset.i, 10);
        openActive();
      });
      el.addEventListener('mousemove', function () {
        setActive(parseInt(el.dataset.i, 10));
      });
    });
  }

  function setActive(i) {
    activeIndex = i;
    list.querySelectorAll('.cp-item').forEach(function (el, j) {
      el.classList.toggle('active', j === i);
    });
  }

  function move(delta) {
    if (!items.length) return;
    var next = (activeIndex + delta + items.length) % items.length;
    setActive(next);
    var el = list.querySelector('.cp-item[data-i="' + next + '"]');
    if (el) el.scrollIntoView({ block: 'nearest' });
  }

  function openActive() {
    if (activeIndex >= 0 && items[activeIndex]) {
      window.location.href = items[activeIndex].url;
    }
  }

  function escapeHtml(s) {
    var div = document.createElement('div');
    div.textContent = s;
    return div.innerHTML;
  }

  document.addEventListener('keydown', function (e) {
    if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
      e.preventDefault();
      overlay && !overlay.hidden ? close() : open();
    } else if (e.key === 'Escape' && overlay && !overlay.hidden) {
      close();
    }
  });

  document.addEventListener('DOMContentLoaded', function () {
    var trigger = document.getElementById('cpTrigger');
    if (trigger) trigger.addEventListener('click', open);
  });
})();
