export function getErrorMessage(err: unknown, fallback: string): string {
  if (!err || typeof err !== "object") return fallback;

  const error = err as Record<string, unknown>;
  const response = error.response as Record<string, unknown> | undefined;
  if (!response) return (error as unknown as Error).message || fallback;

  const data = response.data;
  if (typeof data === "string") return data;

  if (data && typeof data === "object") {
    const body = data as Record<string, unknown>;
    const parts: string[] = [];

    const title = body.title;
    if (typeof title === "string" && title !== "One or more validation errors occurred.") {
      parts.push(title);
    }

    const errors = body.errors as Record<string, string[]> | undefined;
    if (errors && typeof errors === "object") {
      for (const field of Object.keys(errors)) {
        const msgs = errors[field];
        if (Array.isArray(msgs)) {
          for (const m of msgs) {
            parts.push(`${field}: ${m}`);
          }
        }
      }
    }

    if (parts.length > 0) return parts.join("; ");
    if (typeof body.message === "string") return body.message;
    if (typeof title === "string") return title;
  }

  return fallback;
}
