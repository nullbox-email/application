import type { GetAllResponse } from "~/types/dto/mailboxes/get-all-response";

type MailboxListItem = GetAllResponse;

const STATE_KEY = "global-mailboxes";

export function useGlobalMailboxes() {
  const mailboxes = useState<MailboxListItem[]>(STATE_KEY, () => []);
  const loaded = useState<boolean>(`${STATE_KEY}:loaded`, () => false);
  const pending = useState<boolean>(`${STATE_KEY}:pending`, () => false);
  const error = useState<unknown | null>(`${STATE_KEY}:error`, () => null);

  const { getAll } = useMailboxes();

  const refreshMailboxes = async (version: string = "v1") => {
    pending.value = true;
    error.value = null;

    try {
      const result = (await getAll(version)) as unknown;

      if (Array.isArray(result)) {
        mailboxes.value = result as MailboxListItem[];
      } else if (result) {
        mailboxes.value = [result as MailboxListItem];
      } else {
        mailboxes.value = [];
      }

      loaded.value = true;
      return mailboxes.value;
    } catch (err) {
      error.value = err;
      throw err;
    } finally {
      pending.value = false;
    }
  };

  const ensureMailboxes = async (version: string = "v1") => {
    if (loaded.value) {
      return mailboxes.value;
    }

    return await refreshMailboxes(version);
  };

  return {
    mailboxes,
    pending,
    error,
    loaded,
    refreshMailboxes,
    ensureMailboxes,
  };
}

