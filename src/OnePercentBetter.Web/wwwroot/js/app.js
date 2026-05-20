import { showFlashMessages } from './modules/notifications.js';
import { bindConfirmDialogs } from './modules/dialogs.js';
import { initProductTour } from './modules/product-tour.js';

showFlashMessages(window.__flashMessages ?? []);
bindConfirmDialogs();
initProductTour();

if (window.lucide) {
  window.lucide.createIcons();
}
