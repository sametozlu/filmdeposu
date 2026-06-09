(function () {
  const widget = document.getElementById('theme-music-widget');
  if (!widget) return;

  const videoId = widget.dataset.youtubeId;
  if (!videoId) return;

  const btn = document.getElementById('theme-play-btn');
  const label = document.getElementById('theme-music-label');
  const playText = widget.dataset.playText || 'Play';
  const pauseText = widget.dataset.pauseText || 'Pause';
  const playingText = widget.dataset.playingText || 'Playing';

  let player = null;
  let apiReady = false;

  function setUi(playing) {
    btn.setAttribute('aria-pressed', playing ? 'true' : 'false');
    btn.textContent = playing ? '⏸ ' + pauseText : '▶ ' + playText;
    label.textContent = playing ? playingText : playText;
    widget.classList.toggle('is-playing', playing);
  }

  function initPlayer() {
    player = new YT.Player('theme-yt-player', {
      height: '1',
      width: '1',
      videoId: videoId,
      playerVars: {
        autoplay: 0,
        loop: 1,
        playlist: videoId,
        controls: 0,
        modestbranding: 1,
        rel: 0,
        playsinline: 1,
        enablejsapi: 1,
        origin: window.location.origin
      },
      events: {
        onReady: function () {
          apiReady = true;
          widget.classList.remove('is-hidden');
        },
        onStateChange: function (e) {
          if (e.data === YT.PlayerState.PLAYING) {
            player.unMute();
            player.setVolume(55);
            setUi(true);
          } else if (e.data === YT.PlayerState.PAUSED) {
            setUi(false);
          } else if (e.data === YT.PlayerState.ENDED) {
            player.playVideo();
          }
        },
        onError: function () {
          setUi(false);
        }
      }
    });
  }

  btn.addEventListener('click', function () {
    if (!apiReady || !player) return;
    const state = player.getPlayerState();
    if (state === YT.PlayerState.PLAYING || state === YT.PlayerState.BUFFERING) {
      player.pauseVideo();
      return;
    }
    player.unMute();
    player.setVolume(55);
    player.playVideo();
  });

  if (window.YT && window.YT.Player) {
    initPlayer();
  } else {
    const previousReady = window.onYouTubeIframeAPIReady;
    window.onYouTubeIframeAPIReady = function () {
      if (typeof previousReady === 'function') previousReady();
      initPlayer();
    };
    const tag = document.createElement('script');
    tag.src = 'https://www.youtube.com/iframe_api';
    document.head.appendChild(tag);
  }
})();
