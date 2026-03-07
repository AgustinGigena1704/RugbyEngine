// En desarrollo no se habilita caché offline para que los cambios
// se reflejen inmediatamente sin tener que limpiar el service worker.
self.addEventListener('fetch', () => { });
