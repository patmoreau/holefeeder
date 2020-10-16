export function dateFromUtc(date: Date | string | number): Date {
    if (date == null) {
        return null;
    }
    const utc = new Date(date);
    return new Date(utc.getUTCFullYear(), utc.getUTCMonth(), utc.getUTCDate());
}

export function dateToUtc(date: Date | string | number): Date {
    if (date == null) {
        return null;
    }
    const utc = new Date(date);
    utc.setUTCHours(0);
    utc.setUTCMinutes(0);
    utc.setUTCSeconds(0);
    utc.setUTCMilliseconds(0);
    return utc;
}
