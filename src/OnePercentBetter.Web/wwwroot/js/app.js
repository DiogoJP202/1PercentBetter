import { showFlashMessages } from './modules/notifications.js';

showFlashMessages(window.__flashMessages ?? []);

if (window.lucide) {
  window.lucide.createIcons();
}
