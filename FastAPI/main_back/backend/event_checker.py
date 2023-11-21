class EventChecker():
    '''
    하나의 subject에 event는 하나라는 가정으로 구현하였습니다.
    '''
    def __init__(self) -> None:
        self.events = {}

    def get_event(self, subject):
        if subject in self.events.keys():
            return self.events[subject]
        return (None, None, None)

    def add_event(self, curr_event):
        # curr_event = (subject, predicate, object)
        self.events[curr_event[0]] = curr_event[1:]
    
    def remove_event(self, curr_event):
        if curr_event[0] in self.events:
            del self.events[curr_event[0]]
    
    def remove_event_on_subject(self, subject):
        if subject in self.events:
            del self.events[subject]

    def turn_event_to_idle(self, curr_event):
        if curr_event[0] in self.events:
            self.events[curr_event[0]] = (None, None, None)