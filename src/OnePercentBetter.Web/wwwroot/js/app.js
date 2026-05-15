import { showFlashMessages } from './modules/notifications.js';
import { bindConfirmDialogs } from './modules/dialogs.js';

showFlashMessages(window.__flashMessages ?? []);
bindConfirmDialogs();

if (window.lucide) {
  window.lucide.createIcons();
}
