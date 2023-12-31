import { HttpClient } from '@angular/common/http';

export function ImgSrcHookFunction(httpClient: HttpClient, lazyImage: string | ArrayBuffer | undefined | null) {
  const property = Object.getOwnPropertyDescriptor(Image.prototype, 'src');
  const nativeSet: any = property?.set;
  function srcHook(this: any, url: any) {
    const img = this;
    if (url === null || url === undefined || url === '') {
      nativeSet.call(img, url);
      return;
    }
    const base64Regex = /^data:image[/][a-zA-Z*]+;base64/;
    if (base64Regex.test(url) === true) {
      nativeSet.call(img, url);
      return;
    }
    httpClient.get(url, { responseType: 'blob' }).subscribe((blob: any) => {
      const reader = new FileReader();
      reader.readAsDataURL(blob);
      reader.onloadend = function () {
        const base64data = reader.result;
        nativeSet.call(img, base64data);
      };
    });
    if (lazyImage !== undefined && lazyImage !== null) {
      nativeSet.call(img, lazyImage);
    }
  }
  Object.defineProperty(Image.prototype, 'src', {
    set: srcHook
  });
}
