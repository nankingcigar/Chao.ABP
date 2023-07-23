import {
  BaseModalContainerComponent,
  ModalOptions,
  NzModalRef,
  NzModalService,
} from 'ng-zorro-antd/modal';
import { ComponentType } from '@angular/cdk/portal';
import { OverlayRef } from '@angular/cdk/overlay';
import { TemplateRef } from '@angular/core';

type ContentType<T> = ComponentType<T> | TemplateRef<T> | string;
var originattachModalContent: <T, D, R>(
  config: ModalOptions<T, D, R>
) => NzModalRef<T, R> = (NzModalService as any).prototype.attachModalContent;
(NzModalService as any).prototype.attachModalContent = function <T, D, R>(
  componentOrTemplateRef: ContentType<T>,
  modalContainer: BaseModalContainerComponent,
  overlayRef: OverlayRef,
  config: ModalOptions<T>
): NzModalRef<T, R> {
  var modalRef = (originattachModalContent as any).call(
    this,
    componentOrTemplateRef,
    modalContainer,
    overlayRef,
    config
  );
  if (
    modalRef.componentInstance !== undefined &&
    modalRef.componentInstance !== null
  ) {
    Object.assign(<{}>modalRef.componentInstance, config.nzData);
  }
  return modalRef;
};

export * from './chao-antd.module';
