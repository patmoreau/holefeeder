// import { FormControl } from '@angular/forms';

// export function restrictedWords(words: Array<string>) {
//   return (control: FormControl): { [key: string]: any } => {
//     if (!words) {
//       return null;
//     }

//     const invalidWords = words.filter(w => control.value.includes(w));

//     return invalidWords && invalidWords.length > 0
//       ? { restrictedWords: invalidWords.join(', ') }
//       : null;
//   };
// }
// @Directive({
//   selector: '[appForbiddenName]',
//   providers: [{provide: NG_VALIDATORS, useExisting: ForbiddenValidatorDirective, multi: true}]
// })
// export class ForbiddenValidatorDirective implements Validator {
//   @Input('appForbiddenName') forbiddenName: string;
 
//   validate(control: AbstractControl): {[key: string]: any} | null {
//     return this.forbiddenName ? forbiddenNameValidator(new RegExp(this.forbiddenName, 'i'))(control)
//                               : null;
//   }
// }