import React from "react";
import { useDispatch } from "react-redux";
import { initializeNotes } from "../actions";

const Search = () => {
  const dispatch = useDispatch();

  const searchNotes = (event) => {
    event.preventDefault();
      const content = event.target.content.value;
    if (content) {
      event.target.content.value = "";
      dispatch(initializeNotes(content));
    }
  };

  return (
      <form onSubmit={searchNotes}>
      <input
        name="content"
        type="text"
        className="input"
        placeholder="What do you want to search?"
      />
      <button type="submit" className="to-do__add" id="search">
      </button>
    </form>
  );
};

export default Search;
